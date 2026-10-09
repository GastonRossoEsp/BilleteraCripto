<template>
  <section class="lista-container">
    <div class="encabezado">
      <h2>Historial de transacciones</h2>
    </div>

    <div class="filtro">
      <label>Cliente</label>
      <select v-model.number="clienteId">
        <option :value="0">Seleccionar cliente</option>
        <option v-for="cliente in clientes" :key="cliente.id" :value="cliente.id">{{ cliente.nombre }}</option>
      </select>

      <button @click="actualizar">Actualizar</button>
    </div>

    <p v-if="error" class="error">{{ error }}</p>
    <p v-if="transaccionesStore.loading">Cargando transacciones...</p>
    <p v-else-if="!clienteId">Selecciona un cliente para consultar sus movimientos.</p>
    <p v-else-if="transaccionesFiltradas.length === 0">El cliente no tiene transacciones registradas.</p>

    <table v-else>
      <thead>
        <tr>
          <th>ID</th>
          <th>Operacion</th>
          <th>Cripto</th>
          <th>Cantidad</th>
          <th>Dinero</th>
          <th>Fecha</th>
          <th>Acciones</th>
        </tr>
      </thead>

      <tbody>
        <tr v-for="transaccion in transaccionesFiltradas" :key="transaccion.id">
          <td>{{ transaccion.id }}</td>
          <td>{{ transaccion.metodo === "purchase" || transaccion.metodo === "compra" ?"compra" :"venta"}}</td>
          <td>{{ transaccion.codigoCripto.toUpperCase() }}</td>
          <td>{{ formatearCantidad(transaccion.cantCripto) }}</td>
          <td>{{ formatearDinero(transaccion.dinero) }}</td>
          <td>{{ formatearFecha(transaccion.datetime) }}</td>

          <td class="acciones">
            <button @click="verTransaccion(transaccion.id)">Ver</button>
            <button @click="editarTransaccion(transaccion.id)">Editar</button>
            <button @click="eliminarTransaccion(transaccion.id)">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>

    <div v-if="transaccionVista" class="detalle">
      <h3>Detalle de transaccion</h3>

      <p><strong>ID:</strong> {{ transaccionVista.id }}</p>
      <p><strong>Cliente:</strong> {{ transaccionVista.clienteNombre }}</p>
      <p><strong>Criptomoneda:</strong>{{ transaccionVista.codigoCripto.toUpperCase() }}</p>
      <p><strong>Operacion:</strong> {{ transaccionVista.metodo === "purchase" || transaccionVista.metodo === "compra" ?"compra" :"venta"}}</p>
      <p><strong>Cantidad:</strong> {{ formatearCantidad(transaccionVista.cantCripto) }}</p>
      <p><strong>Dinero:</strong> {{ formatearDinero(transaccionVista.dinero) }}</p>
      <p><strong>Fecha:</strong> {{ formatearFecha(transaccionVista.datetime) }}</p>

      <button @click="transaccionVista = null">Cerrar</button>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useTransaccionesStore } from '@/stores/transacciones.store'
import { useClientesStore } from '@/stores/clientes.store'

const emit = defineEmits(["editar"])
const transaccionesStore = useTransaccionesStore()
const clientesStore = useClientesStore()

const clienteId = ref(0)
const transaccionVista = ref(null)
const error = ref("")
const clientes = computed(() => clientesStore.clientes)

const transaccionesFiltradas = computed(() => {
  if (!clienteId.value) {
    return []
  }
  return transaccionesStore.transacciones.filter(transaccion => Number(transaccion.clienteId) === Number(clienteId.value))
})

const cargarTransacciones = async () => {
  error.value = ""
  try {
    await transaccionesStore.obtenerTodas()
    console.log("Transacciones cargadas:", transaccionesStore.transacciones)
  }catch (err) {
    console.error("Error cargando transacciones:", err)
    error.value = err.response?.data?.mensaje || err.message || "Ocurrió un error al cargar las transacciones."
  }
}

const actualizar = async () => {
  await transaccionesStore.obtenerTodas()
}

const verTransaccion = async (id) => {
  try {
    error.value = ""
    transaccionVista.value = await transaccionesStore.obtenerPorId(id)
  } catch (err) {
    console.error(err)
    error.value = "No se pudo obtener la transaccion"
  }
}

const editarTransaccion = async (id) => {
  try {
    error.value = ""
    const transaccion = await transaccionesStore.obtenerPorId(id)
    if (transaccion) {emit("editar", transaccion)}
  } catch (err) {
    console.error(err)
    error.value = "No se pudo obtener la transaccion"
  }
}

const eliminarTransaccion = async (id) => {
  const confirmar = window.confirm("¿Estás seguro de que deseas eliminar esta transacción?")
  if (!confirmar) {return}

  try {
    error.value = ""
    await transaccionesStore.eliminar(id)
    if (transaccionVista.value?.id === id) {transaccionVista.value = null}
  } catch (err) {
    console.error(err)
    error.value = "No se pudo eliminar la transaccion"
  }
}

const formatearCantidad = (cantidad) => {
  return Number(cantidad).toLocaleString("es-AR", {maximumFractionDigits: 8})
}

const formatearDinero = (dinero) => {
  return Number(dinero).toLocaleString("es-AR", {style: "currency", currency: "ARS"})
}

const formatearFecha = (fecha) => {
  if (!fecha) {return "Sin fecha"}

  const fechaConvertida = new Date(fecha)
  if (Number.isNaN(fechaConvertida.getTime())) {return "Fecha inválida"}

  return fechaConvertida.toLocaleString("es-AR", {day: "2-digit", month: "2-digit", year: "numeric", hour: "2-digit", minute: "2-digit"})
}

onMounted(async() =>{
  try {
  await clientesStore.obtenerTodos()
  await cargarTransacciones()
  if (clientes.value.length > 0) {clienteId.value = clientes.value[0].id}
  } catch (err) {
    console.error(err)
  }
})
</script>