import API from './api';

export const crearTransaccion = async (transaccion) => {
  const res = await API.post('/Transaccion', transaccion)
  return res.data
}

export const getTransacciones = async () => {
  const res = await API.get('/Transaccion');
  return res.data;
}

export const getTransaccionById = async (id) => {
  const res = await API.get(`/Transaccion/${id}`)
  return res.data
}

export const actualizarTransaccion = async (id, transaccion) => {
  const res = await API.put(`/Transaccion/${id}`, transaccion)
  return res.data
}

export const eliminarTransaccion = async (id) => {
  await API.delete(`/Transaccion/${id}`)
}