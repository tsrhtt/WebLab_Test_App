<template>
  <div class="overlay" @click.self="closeOverlay">
    <div class="patient-details">
      <div class="patient-info">
        <img :src="patientImage" alt="Patient" />
        <h2>Пациент</h2>
        <h3>{{ direction.patient.fullName || 'Нет данных' }}</h3>
        <div class="patient-data">
          <p><strong>Пол:</strong> {{ direction.patient.sexDescription || 'Нет данных' }}</p>
          <p><strong>Возраст:</strong> {{ direction.patient.age || 'Нет данных' }}</p>
          <p><strong>Дата рождения:</strong> {{ formatDate(direction.patient.birthDate) || 'Нет данных' }}</p>
          <p><strong>Номер идентификации:</strong> {{ direction.patient.identificationNumber || 'Нет данных' }}</p>
        </div>
        <button class="details-button" @click="fetchDetailedData">📐</button>
      </div>
      <div class="additional-info">
        <p><strong>ID:</strong> {{ direction.id || 'Нет данных' }}</p>
        <p><strong>Полное имя пациента:</strong> {{ direction.patientFullName || 'Нет данных' }}</p>
        <p><strong>Лаборатория:</strong> {{ direction.laboratory || 'Нет данных' }}</p>
        <p><strong>Тип анализа:</strong> {{ direction.analysTypeName || 'Нет данных' }}</p>
        <p><strong>Статус:</strong> {{ direction.directionStatus || 'Нет данных' }}</p>
        <p><strong>Дата запроса:</strong> {{ formatDate(direction.requestDate) || 'Нет данных' }}</p>
        <p><strong>Запрошено:</strong> {{ direction.requestedBy || 'Нет данных' }}</p>
        <p><strong>Дата принятия:</strong> {{ direction.acceptedDate ? formatDate(direction.acceptedDate) : 'Нет данных' }}</p>
        <p><strong>Принято:</strong> {{ direction.acceptedBy || 'Нет данных' }}</p>
        <p><strong>Дата выполнения:</strong> {{ direction.readyDate ? formatDate(direction.readyDate) : 'Нет данных' }}</p>
        <p><strong>Комментарии:</strong> {{ direction.laborantComment || 'Нет данных' }}</p>
        <p><strong>Тип биоматериала:</strong> {{ direction.bioMaterialType || 'Нет данных' }}</p>
      </div>
      <div class="direction-status-history">
        <h3>История статусов направления</h3>
        <div v-for="history in direction.directionStatusHistory" :key="history.id" class="history-item">
          <p><strong>Статус:</strong> {{ getStatusDescription(history.directionStatusId) }}</p>
          <p><strong>Дата и время:</strong> {{ formatDate(history.dateTime) }}</p>
          <p><strong>Пользователь:</strong> {{ history.userFio }}</p>
          <p><strong>Комментарий:</strong> {{ history.comment || 'Нет данных' }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    direction: {
      type: Object,
      required: true,
    },
  },
  computed: {
    patientImage() {
      return require('D:/project-practice/Web-app/src/assets/person-7243410_1920.png');
    },
  },
  methods: {
    closeOverlay() {
      this.$emit('close');
    },
    formatDate(date) {
      const options = { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' };
      return new Date(date).toLocaleDateString('ru-RU', options);
    },
    getStatusDescription(statusId) {
      const statusDescriptions = {
        1: 'Заявка',
        2: 'В ожидании',
        3: 'Нет биоматериала',
        4: 'Отказ',
        5: 'Отменено',
        6: 'В работе',
        7: 'Готово',
      };
      return statusDescriptions[statusId] || 'Неизвестный статус';
    },
    async fetchDetailedData() {
      this.$emit('fetch-detailed-data', this.direction.id);
    },
  },
};
</script>

<style scoped>
.overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.patient-details {
  background-color: #f0f8ff;
  padding: 20px;
  border-radius: 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 80%;
  max-width: 600px;
  max-height: 80%;
  overflow-y: auto;
}

.patient-info {
  display: flex;
  flex-direction: column;
  align-items: center;
  background-color: #fff;
  padding: 20px;
  border-radius: 20px;
  width: 100%;
  margin-bottom: 20px;
  position: relative;
}

.patient-info img {
  width: 100px;
  height: 100px;
  border-radius: 50%;
  margin-bottom: 10px;
}

.patient-info h2 {
  margin: 0;
  color: #999;
  font-size: 18px;
}

.patient-info h3 {
  margin: 5px 0 15px;
  color: #333;
  font-size: 24px;
}

.patient-data {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  width: 100%;
}

.patient-data p {
  margin: 5px 0;
  width: 45%;
}

.additional-info {
  width: 100%;
  background-color: #f0f8ff;
  padding: 20px;
  border-radius: 20px;
}

.additional-info p {
  margin: 10px 0;
}

.direction-status-history {
  width: 100%;
  background-color: #f0f8ff;
  padding: 20px;
  border-radius: 20px;
  margin-top: 20px;
}

.direction-status-history h3 {
  margin-top: 0;
}

.history-item {
  margin-bottom: 10px;
  padding: 10px;
  background-color: #fff;
  border-radius: 10px;
}

.details-button {
  position: absolute;
  top: 10px;
  right: 10px;
  background-color: #ADD8E6;
  color: white;
  border: none;
  border-radius: 50%;
  padding: 10px;
  font-size: 20px;
  cursor: pointer;
}
</style>
