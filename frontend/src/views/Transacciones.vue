<script setup>
import TransaccionForm from '@/components/TransaccionForm.vue'
import TransaccionLista from '@/components/TransaccionLista.vue'
</script>

<template>
    <div>
        <h2>Transacciones</h2>
        <TransaccionForm />

        <hr />
        
        <TransaccionLista />
    </div>
</template>
