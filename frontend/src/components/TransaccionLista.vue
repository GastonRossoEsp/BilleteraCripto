<template>
  <div>
    <select v-model="clienteId">
      <option v-for="client in clientes" :value="client.id" :key="client.id">{{ client.nombre }}</option>
    </select>
    <button @click="loadTransacciones">Ver Transacciones</button>

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
        <tr v-for="tx in transacciones" :key="tx.id">
          <td>{{ tx.metodo }}</td>
          <td>{{ tx.codigoCripto }}</td>
          <td>{{ tx.cantCripto }}</td>
          <td>{{ tx.dinero }}</td>
          <td>{{ new Date(tx.datetime).toLocaleString() }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const clienteId = ref(0)
const transacciones = ref([])
const clientes = ref([])

const loadClientes = async () => {
    const response = await axios.get(`https://localhost:7198/api/Cliente`)
    clientes.value = response.data
    clienteId.value = response.data[0]?.id ?? 0 }
const loadTransacciones = async  ()=>{
    const response = await axios.get(`https://localhost:7198/api/Transaccion/${clienteId.value}`);
    transacciones.value = response.data
}
onMounted(() =>{
  loadClientes
})
</script>