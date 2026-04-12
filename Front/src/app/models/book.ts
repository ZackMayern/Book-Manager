import { ConditionType } from "../constants/condition-type";

export interface Book {
  id?: string;
  publisher?: string;
  title?: string;
  author?: string;
  yearOfPublication?: string;
  bookCount?: number;
  isAvailable?: boolean;
  condition: ConditionType;
}