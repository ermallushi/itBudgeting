import React from 'react';

interface ModalProps {
  title: string;
  onClose: () => void;
  children: React.ReactNode;
}

const Modal: React.FC<ModalProps> = ({ title, onClose, children }) => (
  <div style={overlay}>
    <div style={modalBox}>
      <div style={header}>
        <h3 style={{ margin: 0 }}>{title}</h3>
        <button onClick={onClose} style={closeBtn}>✕</button>
      </div>
      <div style={{ padding: '16px' }}>{children}</div>
    </div>
  </div>
);

const overlay: React.CSSProperties = {
  position: 'fixed', top: 0, left: 0, right: 0, bottom: 0,
  background: 'rgba(0,0,0,0.5)', display: 'flex',
  alignItems: 'center', justifyContent: 'center', zIndex: 1000,
};
const modalBox: React.CSSProperties = {
  background: '#fff', borderRadius: 8, minWidth: 400, maxWidth: 600,
  maxHeight: '90vh', overflowY: 'auto', boxShadow: '0 4px 24px rgba(0,0,0,0.2)',
};
const header: React.CSSProperties = {
  display: 'flex', justifyContent: 'space-between', alignItems: 'center',
  padding: '12px 16px', borderBottom: '1px solid #eee',
};
const closeBtn: React.CSSProperties = {
  background: 'none', border: 'none', cursor: 'pointer', fontSize: 18,
};

export default Modal;
