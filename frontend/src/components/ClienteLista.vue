<template>
  <div class="clientes">
    <h3>Clientes registrados</h3>
    <table>
      <thead>
        <tr>
          <th>Nombre</th>
          <th>Email</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="c in store.clientes":key="c.id">
          <td>{{ c.nombre }}</td>
          <td>{{ c.email }}</td>
          <td>
            <button @click="eliminar(c.id)">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useClientesStore } from '@/stores/clientes.store'

const store = useClientesStore()

const eliminar = async (id) => {
  await store.eliminar(id)
}

onMounted(() => {
  store.obtenerTodos()
})
</script>
