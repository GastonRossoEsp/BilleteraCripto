import { defineStore } from "pinia";
import API from "@/services/api";

export const useClientesStore = defineStore("clientes",{
  state: () => ({
    clientes: [],
    cargando: false
  }),

  actions: {
    async obtenerTodos() {
      this.cargando = true
      const res = await API.get('/Cliente')
      this.clientes = res.data
      this.cargando = false
    },

    async crear(cliente) {
      await API.post('/Cliente', cliente)
      await this.obtenerTodos()
    },

    async eliminar(id) {
      await API.delete(`/Cliente/${id}`)
      await this.obtenerTodos()
    }
  }
});  