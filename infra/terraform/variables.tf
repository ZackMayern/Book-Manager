variable "region" {
  description = "AWS region"
  type        = string
}

variable "ecr_repository_name" {
  description = "ECR repository name"
  type        = string
}

variable "eks_cluster_name" {
  description = "EKS cluster name"
  type        = string
}

variable "create_ecr" {
  description = "Set to false when ECR repository already exists"
  type        = bool
  default     = true
}

variable "create_eks" {
  description = "Set to false when EKS cluster already exists"
  type        = bool
  default     = true
}

variable "tags" {
  description = "Additional tags applied to resources"
  type        = map(string)
  default     = {}
}