<template>
    <main>
        <h2>Estado de cartera</h2>
        <p>Consulta las criptomonedas que posee cada cliente y su valoración actual en pesos argentinos.</p>

        <div>
            <label for="clienteCartera">Seleccionar cliente</label>
            <select id="clienteCartera" v-model.number="clienteId" @change="carteraStore.limpiarCartera()">
                <option :value="0" disabled>-- Seleccione un cliente --</option>
                <option v-for="cliente in clientes" :key="cliente.id" :value="cliente.id">{{ cliente.nombre }} - #{{ cliente.id }}</option>
            </select>

            <button @click="consultarCartera" :disabled="cargando || !clienteId">{{ cargando ? "Consultando..." : "Consultar cartera" }}</button>
        </div>
        <p v-if="cargando">Consultando cotizaciones y calculando cartera...</p>
        <p v-else-if="error" role="alert">{{ error }}</p>
        <template v-else-if="cartera">
            <h3>Cliente: {{ cartera.clienteNombre }}</h3>
            <p><strong>Valor total de la cartera:</strong>{{ formatearDinero(cartera.total) }}</p>
            <p><strong>Saldo disponible:</strong>{{ formatearDinero(cartera.saldoDisponible) }}</p>
            <p><strong>Patrimonio total:</strong>{{ formatearDinero(cartera.patrimonioTotal) }}</p>
            <CarteraGrafico :criptomonedas="cartera.criptomonedas" />
            <template v-if="cartera.criptomonedas.length > 0">
                <table>
                    <thead>
                        <tr>
                            <th>Criptomoneda</th>
                            <th>Cantidad disponible</th>
                            <th>Valor actual (ARS)</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="cripto in cartera.criptomonedas" :key="cripto.codigoCripto">
                            <td>{{ cripto.codigoCripto.toUpperCase() }}</td>
                            <td>{{ formatearCantidad(cripto.cantidad) }}</td>
                            <td>{{ formatearDinero(cripto.dinero) }}</td>
                        </tr>
                    </tbody>
                </table>
            </template>
            <p v-else>El cliente no posee criptomonedas en su cartera.</p>
        </template>
        <p v-else-if="!cargando && clientes.length === 0">No hay clientes registrados para consultar.</p>
    </main>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useClientesStore } from '@/stores/clientes.store'
import { useCarteraStore } from '@/stores/cartera.store'
import CarteraGrafico from '@/components/CarteraGrafico.vue'

const clientesStore = useClientesStore()
const carteraStore = useCarteraStore()
const clienteId = ref(0)
const errorLocal = ref("")

const clientes = computed(() => clientesStore.clientes)
const cartera = computed(() => carteraStore.cartera)
const cargando = computed(() => carteraStore.loading)
const error = computed(() => errorLocal.value || carteraStore.error)

const formatearDinero = (valor) => {
    return Number(valor ?? 0).toLocaleString("es-AR", { style: "currency", currency: "ARS" })
}

const formatearCantidad = (cantidad) => {
    return Number(cantidad ?? 0).toLocaleString("es-AR", { minimumFractionDigits: 2, maximumFractionDigits: 8 })
}

const consultarCartera = async () => {
    errorLocal.value = ""
    if (!clienteId.value){
        errorLocal.value = "Debe seleccionar un cliente."
        return
    }
    await carteraStore.obtenerCartera(Number(clienteId.value))
}

onMounted(async () => {
    try {
        await clientesStore.obtenerTodos()
        if (clientes.value.length > 0) {
            clienteId.value = clientes.value[0].id
            await consultarCartera()
        }
    } catch (err) {
        console.error("Error inicializando cartera:", err)
        errorLocal.value = "Ocurrió un error al obtener los clientes."
    }
})
</script>