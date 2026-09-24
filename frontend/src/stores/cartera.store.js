import { defineStore } from "pinia";
import { getCartera } from "@/services/cartera.service";

export const useCarteraStore = defineStore("cartera", {
    state: () => ({
        cartera: null,
        loading: false,
        error: null
    }),

    actions: {
        async obtenerCartera(clienteId) {
            this.loading = true
            this.error = null

            try {
                this.cartera = await getCartera(clienteId)
                return this.cartera
            } catch (error) {
                console.error(error)

                if (error.response?.status === 404) {
                    this.error = "Cliente no encontrado."
                } else {
                    this.error = "No se pudo obtener la cartera."
                }

                this.cartera = null
                return null
            } finally {
                this.loading = false
            }
        },

        limpiarCartera() {
            this.cartera = null
            this.error = null
        }
    }
})