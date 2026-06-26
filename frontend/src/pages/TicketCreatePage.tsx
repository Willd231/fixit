import React from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../api';
import { useNavigate } from 'react-router-dom';

const schema = z.object({
  title: z.string().min(5, 'Title is required'),
  description: z.string().min(10, 'Please provide details'),
  requesterName: z.string().min(2),
  requesterEmail: z.string().email(),
  category: z.string().min(1),
  priority: z.string().min(1)
});

type FormValues = z.infer<typeof schema>;

const TicketCreatePage = () => {
  const { register, handleSubmit, formState } = useForm<FormValues>({ resolver: zodResolver(schema) });
  const qc = useQueryClient();
  const nav = useNavigate();
  const m = useMutation({ mutationFn: (data: any) => api.createTicket(data), onSuccess: () => { qc.invalidateQueries({ queryKey: ['tickets'] }); nav('/tickets'); } });

  const onSubmit = (data: FormValues) => {
    m.mutate(data);
  };

  return (
    <div>
      <h2>Create Ticket</h2>
      <div className="container" style={{maxWidth:720}}>
        <form onSubmit={handleSubmit(onSubmit)}>
          <div style={{display:'grid',gap:8}}>
            <label>Title</label>
            <input {...register('title')} />
            {formState.errors.title && <div style={{color:'red'}}>{formState.errors.title.message}</div>}

            <label>Description</label>
            <textarea {...register('description')} rows={6} />
            {formState.errors.description && <div style={{color:'red'}}>{formState.errors.description.message}</div>}

            <label>Requester Name</label>
            <input {...register('requesterName')} />
            {formState.errors.requesterName && <div style={{color:'red'}}>{formState.errors.requesterName.message}</div>}

            <label>Requester Email</label>
            <input {...register('requesterEmail')} />
            {formState.errors.requesterEmail && <div style={{color:'red'}}>{formState.errors.requesterEmail.message}</div>}

            <label>Category</label>
            <select {...register('category')}>
              <option value="Software">Software</option>
              <option value="Hardware">Hardware</option>
              <option value="Network">Network</option>
              <option value="Account Access">Account Access</option>
              <option value="Security">Security</option>
              <option value="Email">Email</option>
              <option value="Other">Other</option>
            </select>

            <label>Priority</label>
            <select {...register('priority')}>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
              <option value="Critical">Critical</option>
            </select>

            <div style={{display:'flex',gap:8,marginTop:8}}>
              <button type="submit" disabled={Boolean((m as any).isLoading)}>Create</button>
              <button type="button" onClick={()=>nav('/tickets')}>Cancel</button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
};

export default TicketCreatePage;
