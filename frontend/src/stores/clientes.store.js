import { defineStore } from "pinia";
import {
  getClientes,
  getClienteById,
  crearCliente,
  actualizarCliente,
  eliminarCliente
} from "../services/clientes.service";

export const useClientesStore = defineStore("clientes",{
  state: () => ({
    clientes: [],
    clienteSeleccionado: null,
    loading: false,
    error: null
  }),

  actions: {
    async obtenerTodos() {
      this.loading = true
      this.error = null
      try {
        this.clientes = await getClientes()
      } catch (error) {
        console.error("error al obtener clientes: ", error)
        this.error = error.response?.data?.mensaje || error.message ||"No se pudieron obtener los clientes."
        throw error
      } finally {
        this.loading = false
      }
    },

    async obtenerPorId(id) {
      try {
        this.clienteSeleccionado = await getClienteById(id)
        return this.clienteSeleccionado
      } catch (error) {
        this.error = "No se pudo obtener el cliente."
        return null
      }
    },

    async crear(cliente) {
      try {
        const nuevoCliente = await crearCliente(cliente)
        this.clientes.push(nuevoCliente)
        return nuevoCliente
      } catch (error) {
        this.error = "No se pudo crear el cliente."
        return null
      }
    },

    async actualizar(id, cliente) {
      try {
        const clienteActualizado = await actualizarCliente(id, cliente)
        const index = this.clientes.findIndex(c => c.id === id)
        if (index !== -1) {
          this.clientes[index] = clienteActualizado
        }
        return clienteActualizado
      } catch (error) {
        this.error = "No se pudo actualizar el cliente."
        throw error
      }
    },

    async eliminar(id) {
      try {
        await eliminarCliente(id)
        this.clientes = this.clientes.filter(c => c.id !== id)
      } catch (error) {
        this.error = "No se pudo eliminar el cliente."
        throw error
      }
    }
  }
})