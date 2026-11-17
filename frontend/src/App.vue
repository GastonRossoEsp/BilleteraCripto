<template>
  <div id="app">
    <header class="header">
      <div class="header-content">
        <h1>Billetera Cripto</h1>
          <nav>
            <router-link to="/">Inicio</router-link> |
            <router-link to="/transacciones">Transacciones</router-link>
            <router-link to="/clientes">Clientes</router-link>
            <router-link to="/cartera">Cartera</router-link>
          </nav>
      </div>
    </header>
    <main class="main-content">
      <router-view />
    </main>
  </div>
</template>
