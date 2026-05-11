import React from 'react';
import Modal from './Modal';

interface ConfirmDialogProps {
  message: string;
  onConfirm: () => void;
  onCancel: () => void;
}

const ConfirmDialog: React.FC<ConfirmDialogProps> = ({ message, onConfirm, onCancel }) => (
  <Modal title="Confirm" onClose={onCancel}>
    <p>{message}</p>
    <div style={{ display: 'flex', gap: 8, justifyContent: 'flex-end' }}>
      <button onClick={onCancel} style={btnSecondary}>Cancel</button>
      <button onClick={onConfirm} style={btnDanger}>Confirm</button>
    </div>
  </Modal>
);

const btnSecondary: React.CSSProperties = { padding: '6px 16px', cursor: 'pointer' };
const btnDanger: React.CSSProperties = { padding: '6px 16px', cursor: 'pointer', background: '#dc3545', color: '#fff', border: 'none', borderRadius: 4 };

export default ConfirmDialog;
