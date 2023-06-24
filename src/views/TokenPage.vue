<template>
  <div>
    <h1>Token Page</h1>
    <div v-if="error" class="error">{{ error }}</div>
    <div v-else-if="loading" class="loading">Loading...</div>
    <div v-else>
      <table class="user-info-table">
        <tr>
          <th>User</th>
          <td>{{ userInfo.user }}</td>
        </tr>
        <tr>
          <th>Full Name</th>
          <td>{{ userInfo.fullName }}</td>
        </tr>
      </table>
    </div>
  </div>
</template>


<script>
import keycloak from '../boot/keycloak'


export default {
  data() {
    return {
      error: null,
      loading: true,
      userInfo: null
    }
  },
  async beforeMount() {
    try {
      const userInfoStr = localStorage.getItem('userInfo')
      if (userInfoStr) {
        this.userInfo = JSON.parse(userInfoStr)
      }
      this.loading = false
    } catch (error) {
      this.error = error.message
      this.loading = false
    }
  }
}
</script>



<style scoped>
.user-info-table {
  border-collapse: collapse;
  width: 100%;
}

.user-info-table th,
.user-info-table td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: left;
}

.user-info-table th {
  background-color: #f2f2f2;
}

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
