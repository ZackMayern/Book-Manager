terraform {
  required_version = ">= 1.6.0"

  backend "s3" {}

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  region = var.region
}

data "aws_caller_identity" "current" {}

data "aws_availability_zones" "available" {
  state = "available"
}

locals {
  tags = merge(
    {
      Project     = "book-manager"
      ManagedBy   = "terraform"
      Environment = "prod"
    },
    var.tags
  )
}

resource "aws_ecr_repository" "app" {
  count = var.create_ecr ? 1 : 0

  name                 = lower(var.ecr_repository_name)
  image_tag_mutability = "MUTABLE"
  force_delete         = true

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = local.tags
}

resource "aws_vpc" "eks" {
  count = var.create_eks ? 1 : 0

  cidr_block           = "10.0.0.0/16"
  enable_dns_hostnames = true
  enable_dns_support   = true

  tags = merge(local.tags, { Name = "${var.eks_cluster_name}-vpc" })
}

resource "aws_subnet" "eks_public_a" {
  count = var.create_eks ? 1 : 0

  vpc_id                  = aws_vpc.eks[0].id
  cidr_block              = "10.0.1.0/24"
  availability_zone       = data.aws_availability_zones.available.names[0]
  map_public_ip_on_launch = true

  tags = merge(local.tags, { Name = "${var.eks_cluster_name}-public-a" })
}

resource "aws_subnet" "eks_public_b" {
  count = var.create_eks ? 1 : 0

  vpc_id                  = aws_vpc.eks[0].id
  cidr_block              = "10.0.2.0/24"
  availability_zone       = data.aws_availability_zones.available.names[1]
  map_public_ip_on_launch = true

  tags = merge(local.tags, { Name = "${var.eks_cluster_name}-public-b" })
}

resource "aws_internet_gateway" "eks" {
  count = var.create_eks ? 1 : 0

  vpc_id = aws_vpc.eks[0].id

  tags = merge(local.tags, { Name = "${var.eks_cluster_name}-igw" })
}

resource "aws_route_table" "eks" {
  count = var.create_eks ? 1 : 0

  vpc_id = aws_vpc.eks[0].id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.eks[0].id
  }

  tags = merge(local.tags, { Name = "${var.eks_cluster_name}-rt" })
}

resource "aws_route_table_association" "eks_a" {
  count = var.create_eks ? 1 : 0

  subnet_id      = aws_subnet.eks_public_a[0].id
  route_table_id = aws_route_table.eks[0].id
}

resource "aws_route_table_association" "eks_b" {
  count = var.create_eks ? 1 : 0

  subnet_id      = aws_subnet.eks_public_b[0].id
  route_table_id = aws_route_table.eks[0].id
}

resource "aws_iam_role" "eks_cluster" {
  count = var.create_eks ? 1 : 0

  name = "${var.eks_cluster_name}-cluster-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Principal = {
        Service = "eks.amazonaws.com"
      }
    }]
  })

  tags = local.tags
}

resource "aws_iam_role_policy_attachment" "eks_cluster_policy" {
  count = var.create_eks ? 1 : 0

  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSClusterPolicy"
  role       = aws_iam_role.eks_cluster[0].name
}

resource "aws_iam_role" "eks_node" {
  count = var.create_eks ? 1 : 0

  name = "${var.eks_cluster_name}-node-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Principal = {
        Service = "ec2.amazonaws.com"
      }
    }]
  })

  tags = local.tags
}

resource "aws_iam_role_policy_attachment" "eks_worker_node" {
  count = var.create_eks ? 1 : 0

  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSWorkerNodePolicy"
  role       = aws_iam_role.eks_node[0].name
}

resource "aws_iam_role_policy_attachment" "eks_cni" {
  count = var.create_eks ? 1 : 0

  policy_arn = "arn:aws:iam::aws:policy/AmazonEKS_CNI_Policy"
  role       = aws_iam_role.eks_node[0].name
}

resource "aws_iam_role_policy_attachment" "eks_ecr_read" {
  count = var.create_eks ? 1 : 0

  policy_arn = "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly"
  role       = aws_iam_role.eks_node[0].name
}

resource "aws_eks_cluster" "app" {
  count = var.create_eks ? 1 : 0

  name     = var.eks_cluster_name
  role_arn = aws_iam_role.eks_cluster[0].arn

  vpc_config {
    subnet_ids = [
      aws_subnet.eks_public_a[0].id,
      aws_subnet.eks_public_b[0].id
    ]
  }

  depends_on = [
    aws_iam_role_policy_attachment.eks_cluster_policy
  ]

  tags = local.tags
}

resource "aws_eks_node_group" "app" {
  count = var.create_eks ? 1 : 0

  cluster_name    = aws_eks_cluster.app[0].name
  node_group_name = "${var.eks_cluster_name}-ng"
  node_role_arn   = aws_iam_role.eks_node[0].arn
  subnet_ids = [
    aws_subnet.eks_public_a[0].id,
    aws_subnet.eks_public_b[0].id
  ]
  instance_types = ["t3.small"]

  scaling_config {
    desired_size = 2
    min_size     = 2
    max_size     = 2
  }

  depends_on = [
    aws_iam_role_policy_attachment.eks_worker_node,
    aws_iam_role_policy_attachment.eks_cni,
    aws_iam_role_policy_attachment.eks_ecr_read,
    aws_eks_cluster.app
  ]

  tags = local.tags
}

output "ecr_repository_url" {
  description = "ECR repository URL"
  value       = "${data.aws_caller_identity.current.account_id}.dkr.ecr.${var.region}.amazonaws.com/${var.ecr_repository_name}"
}

output "eks_cluster_name" {
  description = "EKS cluster name"
  value       = var.eks_cluster_name
}