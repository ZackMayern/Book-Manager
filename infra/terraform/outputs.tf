output "cluster_name" {
  description = "Cluster name used by deployment"
  value       = var.eks_cluster_name
}

output "repository_name" {
  description = "Repository name used by deployment"
  value       = var.ecr_repository_name
}