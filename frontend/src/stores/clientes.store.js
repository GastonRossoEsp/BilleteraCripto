import { defineStore } from "pinia";
import {
  getClientes,
  getClienteById,
  crearCliente,
  actualizarCliente,
  eliminarCliente,
  cargarSaldoCliente
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
        console.error(error)
        this.error = "No se pudo obtener el cliente."
        throw error
      }
    },

    async crear(cliente) {
      this.error = null
      try {
        const nuevoCliente = await crearCliente(cliente)
        this.clientes.push(nuevoCliente)
        return nuevoCliente
      } catch (error) {
        console.error("Error al crear cliente:", error)
        this.error = error.response?.data?.mensaje || error.response?.data?.message || error.response?.data?.title || "No se pudo crear el cliente."
        throw error
      }
    },

    async actualizar(id, cliente) {
      this.error = null
      try {
        await actualizarCliente(id, cliente)
        await this.obtenerTodos() // se consulta nuevamente la lista de clientes para reflejar los cambios
        return this.clientes.find(c => c.id === Number(id)) ?? null
      } catch (error) {
        console.error("Error al actualizar cliente:", error)
        this.error = error.response?.data?.mensaje || error.response?.data?.message || error.response?.data?.title || "No se pudo actualizar el cliente."
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
    },

    async cargarSaldo(id, monto) {
      try {
        await cargarSaldoCliente(id, monto)
        await this.obtenerTodos() // se consulta nuevamente la lista de clientes para reflejar los cambios
      } catch (error) {
        console.error("Error cargando el saldo:", error)
        throw error
      }
    }
  }
})