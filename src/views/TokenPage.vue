<template>
  <div>
    <h1>Token Page</h1>
    <div v-if="error" class="error">{{ error }}</div>
    <div v-else-if="!receivedToken" class="loading">Loading...</div>
    <div v-else class="success">Token: {{ receivedToken }}</div>
  </div>
</template>


<script>
import keycloak from '../boot/keycloak'

export default {
  data() {
    return {
      error: null,
      receivedToken: null
    }
  },
  async mounted() {
    try {
      const keycloakInstance = await keycloak
      this.receivedToken = keycloakInstance.token
    } catch (error) {
      this.error = error.message
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
