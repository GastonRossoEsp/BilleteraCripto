import API from './api';

export const getClientes = async () => {
    const res = await API.get('/Cliente');
    return res.data;
}

export const getClienteById = async (id) => {
  const res = await API.get(`/Cliente/${id}`)
  return res.data
}

export const crearCliente = async (data) => {
  const res = await API.post('/Cliente', data)
  return res.data
}

export const actualizarCliente = async (id, data) => {
  const res = await API.put(`/Cliente/${id}`, data)
  return res.data
}

export const eliminarCliente = async (id) => {
  await API.delete(`/Cliente/${id}`)
}