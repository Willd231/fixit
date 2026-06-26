export type TicketStatus = 'New' | 'Open' | 'In Progress' | 'Pending' | 'Resolved' | 'Closed';
export type TicketPriority = 'Low' | 'Medium' | 'High' | 'Critical';
export type TicketCategory = 'Hardware' | 'Software' | 'Network' | 'Account Access' | 'Security' | 'Email' | 'Other';

export interface TicketComment {
  id: number;
  ticketId: number;
  authorId: number;
  authorName: string;
  content: string;
  createdAt: string;
}

export interface TicketActivity {
  id: number;
  ticketId: number;
  type: string;
  message: string;
  createdAt: string;
}

export interface Ticket {
  id: number;
  ticketNumber: string;
  title: string;
  description: string;
  requesterName: string;
  requesterEmail: string;
  status: TicketStatus;
  priority: TicketPriority;
  category: TicketCategory;
  assignedTeamId?: number;
  assignedTeamName?: string;
  assignedUserId?: number;
  assignedUserName?: string;
  createdAt: string;
  updatedAt: string;
  resolvedAt?: string | null;
  comments?: TicketComment[];
  activity?: TicketActivity[];
}

export interface User {
  id: number;
  name: string;
  email: string;
  role: 'Administrator' | 'Support Manager' | 'Technician' | 'Requester';
  teamId?: number;
  available?: boolean;
}

export interface Team {
  id: number;
  name: string;
  description?: string;
  technicians?: User[];
}

export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface DashboardStatistics {
  totalOpen: number;
  critical: number;
  awaitingAssignment: number;
  resolvedThisWeek: number;
}

export interface TicketFilterParameters {
  status?: TicketStatus;
  priority?: TicketPriority;
  category?: TicketCategory;
  teamId?: number;
  assignedUserId?: number;
  search?: string;
  page?: number;
  pageSize?: number;
  sort?: string;
  from?: string;
  to?: string;
}
