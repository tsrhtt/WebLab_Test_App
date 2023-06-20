<template>
  <div>
    <h1>Token Page</h1>
    <div v-if="error" class="error">{{ error }}</div>
    <div v-else-if="!token" class="loading">Loading...</div>
    <div v-else class="success">Token: {{ receivedToken }}</div>
  </div>
</template>

<script>
import keycloak from '../router/keycloak'

export default {
  data() {
    return {
      token: null,
      error: null,
      receivedToken: null
    }
  },
  mounted() {
    keycloak()
      .then((keycloak) => {
        keycloak
          .login()
          .then((authenticated) => {
            if (authenticated) {
              keycloak
                .updateToken(5)
                .then(() => {
                  this.token = keycloak.token
                  this.receivedToken = this.token
                })
                .catch((error) => {
                  this.error = error
                })
            } else {
              this.error = 'User authentication failed'
            }
          })
          .catch((error) => {
            this.error = error
          })
      })
      .catch((error) => {
        this.error = error
      })
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
