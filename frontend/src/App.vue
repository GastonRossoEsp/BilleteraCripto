<template>
  <div id="app">
    <header class="header">
      <h1>Billetera Cripto</h1>
      <nav>
        <router-link to="/">Inicio</router-link> |
        <router-link to="/transacciones">Transacciones</router-link>
      </nav>
      <router-view />
    </header>
  </div>
</template>

<style scoped>
nav {
  padding: 1rem;
  background-color: #f2f2f2;
}
nav a {
  margin-right: 1rem;
  text-decoration: none;
}
</style>
