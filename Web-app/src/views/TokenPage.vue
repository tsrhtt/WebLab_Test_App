<template>
  <div>
    <button @click="getDirections">Get Directions</button>
    <table>
      <thead>
        <tr>
          <th>Id</th>
          <th>Patient Full Name</th>
          <th>Sex Description</th>
          <th>Laboratory Name</th>
          <th>Status</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="direction in directions" :key="direction.id">
          <td>{{ direction.id }}</td>
          <td>{{ direction.patientFullName }}</td>
          <td>{{ direction.sexDescription }}</td>
          <td>{{ direction.laboratory }}</td>
          <td>{{ direction.directionStatus }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
export default {
  data() {
    return {
      directions: [],
    };
  },
  methods: {
    async getDirections() {
      try {
        const response = await this.$api.get("direction");
        this.directions = response.data;
      } catch (error) {
        console.error('Failed to fetch directions:', error);
        alert('Failed to fetch directions. Please check the console for more details.');
      }
    },
  },
};
</script>
