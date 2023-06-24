<template>
  <div>
    <h1>Hello, here's your info!</h1>
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
        <tr>
          <th>User Group</th>
          <td>
            <span v-for="(group, index) in userInfo.userGroup" :key="index">
              {{ group }}
              <span v-if="index < userInfo.userGroup.length - 1">, </span>
            </span>
          </td>
        </tr>
        <tr>
          <th>Client URL</th>
          <td>
            <span v-for="(url, index) in userInfo.clientURL" :key="index">
              {{ url }}
              <span v-if="index < userInfo.clientURL.length - 1">, </span>
            </span>
          </td>
        </tr>
      </table>
    </div>
  </div>
</template>

<script>
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
