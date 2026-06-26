import { Ticket, User, Team, TicketComment, TicketActivity, PaginatedResponse, TicketFilterParameters, DashboardStatistics } from '../types';

const randomDelay = (min = 300, max = 900) => new Promise((r) => setTimeout(r, Math.random() * (max - min) + min));

let tickets: Ticket[] = [];
let users: User[] = [];
let teams: Team[] = [];
let nextId = 1000;

function nowISO(offsetDays = 0) {
  const d = new Date();
  d.setDate(d.getDate() + offsetDays);
  return d.toISOString();
}

function seed() {
  users = [
    { id: 1, name: 'Ava Johnson', email: 'ava.johnson@example.com', role: 'Technician', teamId: 1, available: true },
    { id: 2, name: 'Liam Smith', email: 'liam.smith@example.com', role: 'Support Manager', teamId: 2, available: true },
    { id: 3, name: 'Maya Patel', email: 'maya.patel@example.com', role: 'Technician', teamId: 1, available: false },
    { id: 4, name: 'Admin User', email: 'admin@example.com', role: 'Administrator' }
  ];

  teams = [
    { id: 1, name: 'IT Support', description: 'General IT and desktop support', technicians: users.filter((u) => u.teamId === 1) },
    { id: 2, name: 'Network Operations', description: 'Network and connectivity specialists', technicians: users.filter((u) => u.teamId === 2) },
    { id: 3, name: 'Software Support', description: 'Application issues and fixes' },
    { id: 4, name: 'Hardware Support', description: 'Hardware diagnostics and repair' },
    { id: 5, name: 'Security', description: 'Security incidents and investigation' }
  ];

  const sample = [
    {
      title: 'Unable to connect to office Wi-Fi',
      description: 'Several users in the 3rd floor cannot connect to the corporate Wi-Fi. Error says authentication failed.',
      requesterName: 'Emma Walker',
      requesterEmail: 'emma.walker@example.com',
      category: 'Network' as const,
      priority: 'High' as const,
      status: 'Open' as const,
      assignedTeamId: 2,
      assignedTeamName: 'Network Operations'
    },
    {
      title: 'Microsoft Outlook crashes during startup',
      description: 'Outlook closes immediately on startup for user Miguel R.',
      requesterName: 'Carlos Ruiz',
      requesterEmail: 'c.ruiz@example.com',
      category: 'Software' as const,
      priority: 'Medium' as const,
      status: 'New' as const,
      assignedTeamId: 3,
      assignedTeamName: 'Software Support'
    },
    {
      title: 'New employee needs account access',
      description: 'New hire hired today requires access to email, Slack and internal systems.',
      requesterName: 'HR System',
      requesterEmail: 'hr@example.com',
      category: 'Account Access' as const,
      priority: 'High' as const,
      status: 'Pending' as const
    },
    {
      title: 'Printer on second floor is offline',
      description: 'The HP printer shows offline and won\'t accept jobs.',
      requesterName: 'Office Admin',
      requesterEmail: 'office@example.com',
      category: 'Hardware' as const,
      priority: 'Low' as const,
      status: 'Resolved' as const,
      resolvedAt: nowISO(-2)
    },
    {
      title: 'Potential phishing email reported',
      description: 'User reported an email asking for credentials. Needs security review.',
      requesterName: 'Security Team',
      requesterEmail: 'security@example.com',
      category: 'Security' as const,
      priority: 'Critical' as const,
      status: 'In Progress' as const,
      assignedTeamId: 5,
      assignedTeamName: 'Security'
    }
  ];

  tickets = sample.map((s, i) => {
    const id = ++nextId;
    return {
      id,
      ticketNumber: `FIX-${2025}-${id}`,
      title: s.title,
      description: s.description,
      requesterName: s.requesterName,
      requesterEmail: s.requesterEmail,
      status: s.status,
      priority: s.priority,
      category: s.category,
      assignedTeamId: s.assignedTeamId,
      assignedTeamName: s.assignedTeamName,
      createdAt: nowISO(-i - 1),
      updatedAt: nowISO(-i),
      resolvedAt: s.resolvedAt ?? null,
      comments: [],
      activity: [
        { id: id * 10 + 1, ticketId: id, type: 'created', message: 'Ticket created', createdAt: nowISO(-i - 1) }
      ]
    } as Ticket;
  });
}

seed();

export const mockApi = {
  async getTickets(params?: Partial<TicketFilterParameters>): Promise<PaginatedResponse<Ticket>> {
    await randomDelay();
    let items = tickets.slice();
    if (params?.search) {
      const q = params.search.toLowerCase();
      items = items.filter((t) => t.title.toLowerCase().includes(q) || t.description.toLowerCase().includes(q) || t.ticketNumber.toLowerCase().includes(q));
    }
    if (params?.status) items = items.filter((t) => t.status === params.status);
    if (params?.priority) items = items.filter((t) => t.priority === params.priority);
    if (params?.category) items = items.filter((t) => t.category === params.category);
    const page = params?.page ?? 1;
    const pageSize = params?.pageSize ?? 10;
    const total = items.length;
    const start = (page - 1) * pageSize;
    const paged = items.slice(start, start + pageSize);
    return { items: paged, total, page, pageSize };
  },

  async getTicket(id: number) {
    await randomDelay();
    const t = tickets.find((x) => x.id === id);
    if (!t) throw new Error('Not found');
    return t;
  },

  async createTicket(input: Partial<Ticket>) {
    await randomDelay();
    const id = ++nextId;
    const ticket: Ticket = {
      id,
      ticketNumber: `FIX-2025-${id}`,
      title: input.title || 'Untitled',
      description: input.description || '',
      requesterName: input.requesterName || 'Unknown',
      requesterEmail: input.requesterEmail || 'unknown@example.com',
      status: (input.status as any) || 'New',
      priority: (input.priority as any) || 'Medium',
      category: (input.category as any) || 'Other',
      assignedTeamId: input.assignedTeamId,
      assignedTeamName: input.assignedTeamName,
      assignedUserId: input.assignedUserId,
      assignedUserName: input.assignedUserName,
      createdAt: nowISO(),
      updatedAt: nowISO(),
      resolvedAt: null,
      comments: [],
      activity: [{ id: id * 10 + 1, ticketId: id, type: 'created', message: 'Ticket created', createdAt: nowISO() }]
    };
    tickets.unshift(ticket);
    return ticket;
  },

  async updateTicket(id: number, patch: Partial<Ticket>) {
    await randomDelay();
    const idx = tickets.findIndex((t) => t.id === id);
    if (idx === -1) throw new Error('Not found');
    tickets[idx] = { ...tickets[idx], ...patch, updatedAt: nowISO() };
    tickets[idx].activity = tickets[idx].activity || [];
    tickets[idx].activity.push({ id: ++nextId, ticketId: id, type: 'updated', message: 'Ticket updated', createdAt: nowISO() });
    return tickets[idx];
  },

  async deleteTicket(id: number) {
    await randomDelay();
    tickets = tickets.filter((t) => t.id !== id);
    return true;
  },

  async addComment(ticketId: number, authorId: number, authorName: string, content: string): Promise<TicketComment> {
    await randomDelay();
    const ticket = tickets.find((t) => t.id === ticketId);
    if (!ticket) throw new Error('Ticket not found');
    const comment: TicketComment = { id: ++nextId, ticketId, authorId, authorName, content, createdAt: nowISO() };
    ticket.comments = ticket.comments || [];
    ticket.comments.push(comment);
    ticket.activity = ticket.activity || [];
    ticket.activity.push({ id: ++nextId, ticketId, type: 'comment', message: `Comment by ${authorName}`, createdAt: nowISO() } as TicketActivity);
    return comment;
  },

  async getTeams(): Promise<Team[]> {
    await randomDelay();
    return teams;
  },

  async getUsers(): Promise<User[]> {
    await randomDelay();
    return users;
  },

  async getDashboardStats(): Promise<DashboardStatistics> {
    await randomDelay();
    const totalOpen = tickets.filter((t) => t.status !== 'Resolved' && t.status !== 'Closed').length;
    const critical = tickets.filter((t) => t.priority === 'Critical').length;
    const awaitingAssignment = tickets.filter((t) => !t.assignedTeamId).length;
    const resolvedThisWeek = tickets.filter((t) => t.resolvedAt && new Date(t.resolvedAt) > new Date(Date.now() - 7 * 24 * 3600 * 1000)).length;
    return { totalOpen, critical, awaitingAssignment, resolvedThisWeek };
  }
};

export default mockApi;
