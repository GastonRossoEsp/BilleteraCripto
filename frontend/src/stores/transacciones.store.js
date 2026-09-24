import { defineStore } from "pinia";
import {
  crearTransaccion,
  getTransacciones,
  getTransaccionById,
  actualizarTransaccion,
  eliminarTransaccion
} from "@/services/transacciones.service";

export const useTransaccionesStore = defineStore("transacciones", {
  state: () => ({
    transacciones: [],
    transaccionSeleccionada: null,
    loading: false,
    error: null
  }),

  actions: {
    async obtenerTodas() {
      this.loading = true
      this.error = null
      try {
        this.transacciones = await getTransacciones()
      } catch (error) {
        this.error = "No se pudieron obtener las transacciones."
      } finally {
        this.loading = false
      }
    },

    async obtenerPorId(id){
      try {
        this.transaccionSeleccionada = await getTransaccionById(id)
        return this.transaccionSeleccionada
      } catch (error){
        console.error(error)
        this.error = "No se pudo obtener la transaccion"
        return null
      }
    },

    async crear(transaccion) {
      try {
        const nuevaTransaccion = await crearTransaccion(transaccion)
        this.transacciones.push(nuevaTransaccion)
        return nuevaTransaccion
      } catch (error) {
        console.error(error)
        this.error = "No se pudo crear la transaccion."
        throw error
      }
    },

    async actualizar(id, transaccion) {
      try {
        await actualizarTransaccion(id, transaccion)
        await this.obtenerTodas()
      } catch (error) {
        console.error(error)
        this.error = "No se pudo actualizar la transaccion."
        throw error
      }
    },

    async eliminar(id) {
      try {
        await eliminarTransaccion(id)
        this.transacciones = this.transacciones.filter(t => t.id !== id)
      } catch (error) {
        console.error(error)
        this.error = "No se pudo eliminar la transaccion."
        throw error
      }
    }
  }
});