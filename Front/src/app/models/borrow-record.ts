export interface BorrowRecord {
  id: string;
  userId: string;
  userFullName: string;
  bookId: string;
  bookTitle: string;
  borrowedAt: string;
  dueDate: string;
  returnedAt?: string;
  status: 'Active' | 'Overdue' | 'Returned' | 'Renewed';
  renewCount: number;
}