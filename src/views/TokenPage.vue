<template>
  <div>
    <h1>Token Page</h1>
    <div v-if="error" class="error">{{ error }}</div>
    <div v-else-if="loading" class="loading">Loading...</div>
    <div v-else class="success" v-html="'Token: ' + receivedToken"></div>
  </div>
</template>




<script>
import keycloak from '../boot/keycloak'

export default {
  data() {
    return {
      error: null,
      receivedToken: null,
      loading: true
    }
  },
  async beforeMount() {
    try {
      const keycloakInstance = await keycloak
      this.receivedToken = keycloakInstance.token
      this.loading = false
    } catch (error) {
      this.error = error.message
      this.loading = false
    }
  }
}
</script>



<style scoped>
.error {
  color: red;
}
.loading {
  color: blue;
}
.success {
  color: green;
}
</style>
