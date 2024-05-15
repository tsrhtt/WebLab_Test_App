<template>
  <div>
    <button @click="getDirections">Получить направления</button>
    <h2>Запросы</h2>
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Полное имя пациента</th>
          <th>Лаборатория</th>
          <th>Тип анализа</th>
          <th>Статус</th>
          <th>Дата запроса</th>
          <th>Запрошено</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="request in requests" :key="request.id">
          <td>{{ request.id }}</td>
          <td>{{ request.patientFullName }}</td>
          <td>{{ request.laboratory }}</td>
          <td>{{ request.analysTypeName }}</td>
          <td>{{ request.directionStatus }}</td>
          <td>{{ formatDate(request.requestDate) }}</td>
          <td>{{ request.requestedBy }}</td>
        </tr>
      </tbody>
    </table>

    <h2>Направления</h2>
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Полное имя пациента</th>
          <th>Лаборатория</th>
          <th>Тип анализа</th>
          <th>Статус</th>
          <th>Дата запроса</th>
          <th>Запрошено</th>
          <th>Дата принятия</th>
          <th>Принято</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="direction in filteredDirections"
          :key="direction.id"
          :style="rowStyle(direction)"
        >
          <td>{{ direction.id }}</td>
          <td>{{ direction.patientFullName }}</td>
          <td>{{ direction.laboratory }}</td>
          <td>{{ direction.analysTypeName }}</td>
          <td>{{ direction.directionStatus }}</td>
          <td>{{ formatDate(direction.requestDate) }}</td>
          <td>{{ direction.requestedBy }}</td>
          <td>{{ direction.acceptedDate ? formatDate(direction.acceptedDate) : 'Не принято' }}</td>
          <td>{{ direction.acceptedBy ? direction.acceptedBy : '-' }}</td>
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
  computed: {
    requests() {
      return this.directions.filter(direction => direction.directionStatusId === 1);
    },
    filteredDirections() {
      return this.directions.filter(direction => direction.directionStatusId !== 1);
    },
  },
  methods: {
    async getDirections() {
      try {
        const response = await this.$api.get("direction");
        this.directions = this.processApiResponse(response.data); // Process the nested data structure
      } catch (error) {
        console.error('Не удалось получить направления:', error);
        alert('Не удалось получить направления. Пожалуйста, проверьте консоль для получения подробной информации.');
      }
    },
    processApiResponse(data) {
      // Flatten the nested $values structure
      return data.$values.map(direction => {
        return {
          ...direction,
          directionStatusHistory: direction.directionStatusHistory.$values,
          indicators: direction.indicators.$values
        };
      });
    },
    formatDate(date) {
      const options = { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' };
      return new Date(date).toLocaleDateString('ru-RU', options);
    },
    rowStyle(direction) {
      if (direction.directionStatusId === 3) {
        return { backgroundColor: '#CD4A4C66' };
      } else if (direction.directionStatusId === 7) {
        return { backgroundColor: '#98FB9866' };
      }
      return {};
    }
  },
};
</script>

<style scoped>
table {
  width: 100%;
  border-collapse: collapse;
}

th, td {
  border: 1px solid #ddd;
  padding: 8px;
}

th {
  background-color: #f2f2f2;
}

h2 {
  margin-top: 20px;
}

button {
  margin-bottom: 20px;
}
</style>
