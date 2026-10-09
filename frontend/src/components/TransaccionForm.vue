<template>
  <section class="form-container">
    <h2>
      {{ editando ? 'Editar Transacción' : 'Nueva Transacción' }}
    </h2>
    <p v-if="editando" class="info">
      La edición es directa: el backend no recalcula el dinero de la operación.
    </p>

    <form @submit.prevent="guardar">
      <div class="campo">
        <label>Cliente</label>
        <select v-model.number="transaccion.clienteId" :disabled="editando || cargandoClientes" required>
          <option :value="0" disabled>{{ cargandoClientes ? "Cargando clientes..." : "Seleccionar cliente" }}</option>
          <option v-for="cliente in clientes" :key="cliente.id" :value="cliente.id">{{ cliente.nombre }}</option>
        </select>
      </div>

      <div class="campo">
        <label>Criptomoneda</label>

        <select v-model="transaccion.codigoCripto" required>
          <option value="btc">Bitcoin (BTC)</option>
          <option value="eth">Ethereum (ETH)</option>
          <option value="usdt">USD Tether (USDT)</option>
        </select>
      </div>

      <div class="campo">
        <label>Operación</label>
        <select v-model="transaccion.metodo" required>
          <option value="purchase">Compra</option>
          <option value="sale">Venta</option>
        </select>
      </div>

      <div class="campo">
        <label>Cantidad</label>
        <input v-model.number="transaccion.cantCripto" type="number" step="0.00000001" min="0.00000001" required />
      </div>

      <div class="campo">
        <label>Fecha y hora</label>
        <input v-model="transaccion.datetime" type="datetime-local" required />
      </div>

      <p v-if="error" class="error">{{ error }}</p>

      <div class="botones">
        <button type="submit">{{ editando ? "Guardar cambios" : "Registrar transaccion"}}</button>
        <button v-if="editando" type="button" @click="cancelar">Cancelar</button>
      </div>

    </form>
  </section>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useClientesStore } from '@/stores/clientes.store'
import { useTransaccionesStore } from '@/stores/transacciones.store'

const props = defineProps({
  transaccionEditar: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(["guardada", "cancelar"])
const clientesStore = useClientesStore()
const transaccionesStore = useTransaccionesStore()
const clientes = computed(() => clientesStore.clientes)
const error = ref("")

const cargandoClientes = ref(false)

const obtenerFechaActual = () => {
  const ahora = new Date()
  const anio = ahora.getFullYear()
  const mes = String(ahora.getMonth() + 1).padStart(2, '0')
  const dia = String(ahora.getDate()).padStart(2, '0')
  const hora = String(ahora.getHours()).padStart(2, '0')
  const minutos = String(ahora.getMinutes()).padStart(2, '0')
  return `${anio}-${mes}-${dia}T${hora}:${minutos}`
}

const estadoInicial = () => ({
  clienteId: 0,
  codigoCripto: 'btc',
  metodo: 'purchase',
  cantCripto: 0,
  datetime: obtenerFechaActual()
})

const transaccion = ref(estadoInicial())
const editando = computed(() => {
  return props.transaccionEditar !== null})

const cargarTransaccion = (datos) => {
  transaccion.value = {
    clienteId: datos.clienteId,
    codigoCripto: datos.codigoCripto,
    metodo: datos.metodo === "compra" ? "purchase" : datos.metodo === "venta" ? "sale" : datos.metodo,
    cantCripto: datos.cantCripto,
    datetime: datos.datetime ? datos.datetime.substring(0, 16) : obtenerFechaActual()
  }
}

const limpiarFormulario = () => {
  transaccion.value = estadoInicial()
  error.value = ""
}

watch(() => props.transaccionEditar,(nuevaTransaccion) => {
  if(nuevaTransaccion) { cargarTransaccion(nuevaTransaccion) }
  else { limpiarFormulario() }
},
{ immediate: true })

const obtenerMensajeError = (err) => {
  return (
    err.response?.data?.mensaje || err.response?.data || err.message || "Ocurrio un error al procesar la transaccion."
  )
}

const guardar = async () => {
  error.value = ""

  if (!transaccion.value.clienteId) {
    error.value = "Debe seleccionar un cliente."
    return
  }

  if (!transaccion.value.codigoCripto) {
    error.value = "Debe seleccionar una criptomoneda."
    return
  }

  if (!transaccion.value.metodo) {
    error.value = "Debe seleccionar un metodo de operacion."
    return
  }
   if (transaccion.value.cantCripto <= 0) {
    error.value = "Debe ingresar una cantidad de criptomoneda mayor a cero."
    return
  }

  try {
    if (editando.value) {
      await transaccionesStore.actualizar(props.transaccionEditar.id,{
        codigoCripto: transaccion.value.codigoCripto,
        metodo: transaccion.value.metodo,
        cantCripto: transaccion.value.cantCripto,
        datetime: transaccion.value.datetime
      })
    } else {
      await transaccionesStore.crear({
        clienteId: transaccion.value.clienteId,
        codigoCripto: transaccion.value.codigoCripto,
        metodo: transaccion.value.metodo,
        cantCripto: transaccion.value.cantCripto,
        datetime: transaccion.value.datetime
      })
    }

    limpiarFormulario()
    emit("guardada")
  } catch (err) {
    console.error("Error al guardar la transaccion:" ,err)
    error.value = obtenerMensajeError(err)
  }
}

const cancelar = () => {
  limpiarFormulario()
  emit("cancelar")
}

const cargarClientes = async () => {
  cargandoClientes.value = true
  error.value=""

  try{
    await clientesStore.obtenerTodos()
    console.log("Clientes cargados:",clientesStore.clientes)
    if(!editando.value && clientesStore.clientes.length > 0 && !transaccion.value.clienteId){
      transaccion.value.clienteId = clientesStore.clientes[0].id
    }
  } catch (err){
    console.error("Error cargando clientes:", err)
    error.value = "No se pudieron cargar los clientes."
  } finally {cargandoClientes.value = false}
}

onMounted(() => {
  cargarClientes()
})
</script>