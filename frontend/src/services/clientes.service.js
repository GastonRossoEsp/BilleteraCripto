import API from './api';

export const getClientes = async () => {
    const res = await API.get('/Cliente');
    return res.data;
}

export const getClienteById = async (id) => {
  const res = await API.get(`/Cliente/${id}`)
  return res.data
}

export const crearCliente = async (cliente) => {
  const res = await API.post('/Cliente', cliente)
  return res.data
}

export const actualizarCliente = async (id, cliente) => {
  const res = await API.put(`/Cliente/${id}`, cliente)
  return res.data
}

export const eliminarCliente = async (id) => {
  await API.delete(`/Cliente/${id}`)
}