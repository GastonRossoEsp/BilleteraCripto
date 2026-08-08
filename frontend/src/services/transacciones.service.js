import API from './api';

export const crear = async (data) => 
(await API.post('/Transaccion', data)).data;

export const getPorCliente = async (clienteId) =>
    (await API.get(`/Transaccion/${clienteId}`)).data;

export const getById = async (id) =>
  (await API.get(`/Transaccion/${id}`)).data

export const actualizar = async (id, data) =>
  (await API.put(`/Transaccion/${id}`, data)).data

export const eliminar = async (id) =>
  await API.delete(`/Transaccion/${id}`)