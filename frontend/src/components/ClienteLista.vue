<template>
  <section class="table-panel">
    <h2>Clientes registrados</h2>
    <p v-if="store.loading">Cargando clientes...</p>
    <p v-else-if="store.clientes.length === 0">Todavía no hay clientes registrados.</p>
    <p v-if="error" role="alert">{{ error }}</p>

    <table v-if="store.clientes.length > 0">
      <thead>
        <tr>
          <th>ID</th>
          <th>Nombre</th>
          <th>Email</th>
          <th>Saldo disponible</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="cliente in store.clientes" :key="cliente.id">
          <td>{{ cliente.id }}</td>
          <td>{{ cliente.nombre }}</td>
          <td>{{ cliente.email }}</td>
          <td>{{ formatearSaldo(cliente.saldo) }}</td>
          <td class="acciones">
            <button @click="emitirEdicion(cliente)">Editar</button>
            <button @click="eliminar(cliente)" :disabled="eliminandoId === cliente.id">{{ eliminandoId === cliente.id ? "Eliminando..." : "Eliminar" }}</button>
            <button @click="cargarSaldo(cliente)">Cargar saldo</button>
          </td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useClientesStore } from '@/stores/clientes.store'

const emit = defineEmits(["editar"])
const store = useClientesStore()
const error = ref("")
const eliminandoId = ref(null)

const formatearSaldo = (saldo) => {
  return Number(saldo ?? 0).toLocaleString("es-AR", { style: "currency", currency: "ARS" })
}

const emitirEdicion = (cliente) => {
  error.value = ""
  emit("editar", cliente)
}

const eliminar = async (cliente) => {
  const confirmar = window.confirm(`¿Está seguro que desea eliminar al cliente "${cliente.nombre}"? Esta acción no se puede deshacer.`)
  if (!confirmar) return
  error.value = ""
  eliminandoId.value = cliente.id
  try {
    await store.eliminar(cliente.id)
  } catch (err) {
    console.error("Error eliminando cliente:", err)
    const data = err.response?.data
    error.value = (typeof data === "string" ? data : data?.mensaje || data?.message || data?.title) ||
    (!err.response ? "No se pudo conectar con el servidor." : "No se pudo eliminar el cliente. Puede tener transacciones asociadas.")
  } finally {
    eliminandoId.value = null
  }
}

const cargarSaldo = async (cliente) => {
  const entrada = window.prompt(`¿Cuantos pesos queres cargar a "${cliente.nombre}"?`)
  if (entrada === null) return // Cancelado
  const monto = Number(entrada.trim().replace(',', '.'))
  if (!Number.isFinite(monto) || monto <= 0) {
    window.alert("Debe ingresar un monto válido mayor a cero.")
    return
  }
  try {
    await store.cargarSaldo(cliente.id, monto)
    window.alert(`Se cargó correctamente $${monto.toLocaleString("es-AR")} al cliente "${cliente.nombre}".`)
  } catch (err) {
    console.error("Error cargando saldo:", err)
    window.alert(err.response?.data?.mensaje || "Ocurrió un error al cargar el saldo del cliente.")
  }
}

onMounted(async () => {
  try {
    await store.obtenerTodos()
  } catch (err) {
    error.value = "Ocurrió un error al obtener los clientes."
  }
})
</script>

<style scoped>
.table-panel {
  background: white;
  padding: 22px;
  border: 1px solid #e0e4e8;
  border-radius: 6px;
  overflow-x: auto;
}

.table-panel h2{
  margin-bottom: 16px;
}

.table-panel table {
  margin-bottom: 0;
}

.acciones {
  min-width: 270px;
  white-space: nowrap;
}

.acciones button {
  margin: 3px 3px 3px 0;
}

.acciones button:nth-child(2) {
  background-color: #a94442;
}

.acciones button:nth-child(2):hover {
  background-color: #873936;
}

.table-panel [role="alert"] {
  color: #b42318;
}
</style>