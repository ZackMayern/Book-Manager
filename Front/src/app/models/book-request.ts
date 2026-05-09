export interface BookRequest {
  id: string;
  userId: string;
  userFullName: string;
  title: string;
  author: string;
  reason: string;
  status: 'Pending' | 'Approved' | 'Rejected';
  adminMessage: string;
  createdAt: string;
}