import { defineStore } from "pinia";
import api from "@/services/api";

export const useTransaccionesStore = defineStore("transacciones", {
  state: () => ({
    transacciones: [],
    cargando: false
  }),

  actions: {
    async obtenerTodas() {
      const res = await api.get('/Transaccion')
      this.transacciones = res.data
    },

    async crear(transaccion) {
      await api.post('/Transaccion', transaccion)
      await this.obtenerTodas()
    },

    async eliminar(id) {
      await api.delete(`/Transaccion/${id}`)
      await this.obtenerTodas()
    }
  }
});