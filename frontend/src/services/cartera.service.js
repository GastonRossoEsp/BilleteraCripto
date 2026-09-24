import API from "./api"

export const getCartera = async (clienteId) => {
    const res = await API.get(`/Cliente/${clienteId}/Cartera`);
    return res.data;
}