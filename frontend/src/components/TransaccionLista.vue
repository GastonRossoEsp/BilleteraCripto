<template>
  <div>
    <select v-model="clienteId">
      <option
        v-for="client in clientes"
        :key="client.id"
        :value="client.id"
        >
        {{ client.nombre }}
      </option>
    </select>

    <button @click="loadTransacciones">
      Ver transacciones
    </button>

    <table v-if="transacciones.length">
      <thead>
        <tr>
          <th>Metodo</th>
          <th>Cripto</th>
          <th>Cantidad</th>
          <th>ARS</th>
          <th>Fecha</th>
        </tr>
      </thead>

      <tbody>
        <tr
        v-for="t in transacciones"
        :key="t.id"
        >
          <td>{{ t.metodo }}</td>
          <td>{{ t.codigoCripto }}</td>
          <td>{{ t.cantCripto }}</td>
          <td>{{ t.dinero }}</td>
          <td>
            {{ new Date(t.datetime).toLocaleString() }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useTransaccionesStore } from '@/stores/transacciones.store'
import { useClientesStore } from '@/stores/clientes.store'

const transStore = useTransaccionesStore()
const clientesStore = useClientesStore()

const clienteId = ref(0)
const clientes = clientesStore.clientes
const transacciones = transStore.transacciones

const loadClientes = async () => {
  try {
    const response = await axios.get('https://localhost:5225/api/Cliente')
    clientes.value = response.data
    clienteId.value = response.data[0]?.id ?? 0
  }
  catch (error) {
    console.error(error)
  }
}

const loadTransacciones = async () => {
  await transStore.obtenerTodas()
}

onMounted(async() =>{
  await clientesStore.obtenerTodos()
  clienteId.value = clientes[0]?.id ?? 0
})
</script>