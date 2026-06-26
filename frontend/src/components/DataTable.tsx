import React from 'react';

const DataTable = ({ columns, data }: { columns: { key: string; label: string; render?: (row:any)=>React.ReactNode }[]; data: any[] }) => {
  return (
    <table className="table">
      <thead>
        <tr>
          {columns.map((c) => (
            <th key={c.key}>{c.label}</th>
          ))}
        </tr>
      </thead>
      <tbody>
        {data.map((row, idx) => (
          <tr key={row.id || idx}>
            {columns.map((c) => (
              <td key={c.key}>{c.render ? c.render(row) : (row as any)[c.key]}</td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );
};

export default DataTable;
