<template>
  <div class="overlay" @click.self="closeOverlay">
    <div class="patient-details">
      <div class="header">
        <button v-if="!localIsEditing" @click="editDirection" class="action-button">Редактировать</button>
        <button v-if="localIsEditing" @click="saveChanges" class="save-button">Сохранить изменения</button>
        <button v-if="localIsEditing" @click="confirmDelete" class="delete-button">Удалить</button>
        <button v-if="!localIsEditing && localDirection.directionStatusId === 1" @click="acceptDirection" class="action-button">Принять</button>
        <button v-if="!localIsEditing" @click="fetchDetailedData" class="action-button">Получить подробные данные</button>
      </div>
      <div class="patient-info">
        <img :src="patientImage" alt="Пациент" />
        <h2>Пациент</h2>
        <h3>{{ localDirection.patient.fullName || 'Нет данных' }}</h3>
        <div class="patient-data">
          <p><strong>Пол:</strong>
            <span v-if="!localIsEditing">{{ localDirection.patient.sexDescription || 'Нет данных' }}</span>
            <select v-else v-model="localDirection.patient.sexDescription" @change="updateSex">
              <option value="Мужской">Мужской</option>
              <option value="Женский">Женский</option>
            </select>
          </p>
          <p><strong>Возраст:</strong> {{ localDirection.patient.age || 'Нет данных' }}</p>
          <p><strong>Дата рождения:</strong>
            <span v-if="!localIsEditing">{{ formatDate(localDirection.patient.birthDate) || 'Нет данных' }}</span>
            <input v-else type="date" v-model="localDirection.patient.birthDate" @change="updateAge" :max="currentDate" />
          </p>
          <p><strong>Идентификационный номер:</strong>
            <span v-if="!localIsEditing">{{ localDirection.patient.identificationNumber || 'Нет данных' }}</span>
            <input v-else v-model="localDirection.patient.identificationNumber" />
          </p>
        </div>
      </div>
      <div class="additional-info">
        <p><strong>ID:</strong> {{ localDirection.id || 'Нет данных' }}</p>
        <p><strong>Лаборатория:</strong>
          <span v-if="!localIsEditing">{{ localDirection.laboratory || 'Нет данных' }}</span>
          <input v-else v-model="localDirection.laboratory" />
        </p>
        <p><strong>Тип анализа:</strong> {{ localDirection.analysTypeName || 'Нет данных' }}</p>
        <p><strong>Статус:</strong> {{ localDirection.directionStatus || 'Нет данных' }}</p>
        <p><strong>Дата запроса:</strong> {{ formatDate(localDirection.requestDate) || 'Нет данных' }}</p>
        <p><strong>Запрошено:</strong> {{ localDirection.requestedBy || 'Нет данных' }}</p>
        <div v-if="localDirection.directionStatusId === 1 || localDirection.directionStatusId === 6 || localDirection.directionStatusId === 3">
          <p><strong>Дата приёма:</strong> {{ formatDate(localDirection.acceptedDate) || 'Нет данных' }}</p>
          <p><strong>Принято:</strong> {{ localDirection.acceptedBy || 'Нет данных' }}</p>
          <p><strong>Комментарий лаборанта:</strong>
            <span v-if="!localIsEditing">{{ localDirection.laborantComment || 'Нет данных' }}</span>
            <input v-else v-model="localDirection.laborantComment" />
          </p>
          <p><strong>Дата взятия образца:</strong>
            <span v-if="!localIsEditing">{{ formatDate(localDirection.samplingDate) || 'Нет данных' }}</span>
            <span v-else>
              <input type="datetime-local" v-model="localDirection.samplingDate" :min="currentDate" />
              <button @click="updateSamplingDate" class="update-button">Обновить</button>
            </span>
          </p>
          <p><strong>Номер образца:</strong>
            <span v-if="!localIsEditing">{{ localDirection.sampleNumber || 'Нет данных' }}</span>
            <input v-else v-model="localDirection.sampleNumber" />
          </p>
          <p><strong>Доктор взятия образца:</strong>
            <span v-if="!localIsEditing">{{ localDirection.samplingDoctorFio || 'Нет данных' }}</span>
            <input v-else v-model="localDirection.samplingDoctorFio" />
          </p>
          <p><strong>Количество биоматериала:</strong>
            <span v-if="!localIsEditing">{{ localDirection.bioMaterialCount || 'Нет данных' }}</span>
            <input v-else v-model="localDirection.bioMaterialCount" />
          </p>
          <p><strong>Тип биоматериала:</strong>
            <span>{{ localDirection.bioMaterialType || 'Нет данных' }}</span>
          </p>
          <p><strong>Подразделение:</strong>
            <span v-if="!localIsEditing">{{ localDirection.departmentName || 'Нет данных' }}</span>
            <input v-else v-model="localDirection.departmentName" />
          </p>
        </div>
        <div v-if="localDirection.directionStatusId === 6 || localDirection.directionStatusId === 3">
          <p><strong>Врач лаборант:</strong>
            <span v-if="!localIsEditing">{{ localDirection.doctorLabDiagnosticFio || 'Нет данных' }}</span>
            <input v-else v-model="localDirection.doctorLabDiagnosticFio" />
          </p>
          <p><strong>Врач фельдшер лаборант:</strong>
            <span v-if="!localIsEditing">{{ localDirection.doctorFeldsherLaborantFio || 'Нет данных' }}</span>
            <input v-else v-model="localDirection.doctorFeldsherLaborantFio" />
          </p>
          <p><strong>Врач биолог:</strong>
            <span v-if="!localIsEditing">{{ localDirection.doctorBiologFio || 'Нет данных' }}</span>
            <input v-else v-model="localDirection.doctorBiologFio" />
          </p>
        </div>
        <p v-if="localDirection.directionStatusId === 7"><strong>Дата готовности:</strong>
          <span v-if="!localIsEditing">{{ formatDate(localDirection.readyDate) || 'Нет данных' }}</span>
          <input v-else type="datetime-local" v-model="localDirection.readyDate" />
        </p>
      </div>
      <div class="direction-status-history">
        <h3>История статусов</h3>
        <div v-for="history in localDirection.directionStatusHistory" :key="history.id" class="history-item">
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
    isEditing: {
      type: Boolean,
      default: false,
    }
  },
  data() {
    return {
      localIsEditing: this.isEditing,
      localDirection: { ...this.direction },
      originalSamplingDate: this.direction.samplingDate // Store the original sampling date
    };
  },
  computed: {
    patientImage() {
      return require('D:/project-practice/Web-app/src/assets/person-7243410_1920.png');
    },
    currentDate() {
      return new Date().toISOString().slice(0, 16);
    }
  },
  methods: {
    closeOverlay() {
      this.$emit('close');
    },
    formatDate(date) {
      if (!date) return 'Нет данных';
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
      this.$emit('fetch-detailed-data', this.localDirection.id);
    },
    editDirection() {
      this.localIsEditing = true;
    },
    async saveChanges() {
      // Convert dates to UTC before saving
      this.localDirection.onDate = new Date(this.localDirection.onDate).toISOString();
      if (this.localDirection.samplingDate && this.localDirection.samplingDate !== this.originalSamplingDate) {
        this.localDirection.samplingDate = new Date(this.localDirection.samplingDate).toISOString();
        this.localDirection.samplingDateStr = this.localDirection.samplingDate;
      }
      this.localDirection.patient.birthDate = new Date(this.localDirection.patient.birthDate).toISOString();
      if (this.localDirection.readyDate) {
        this.localDirection.readyDate = new Date(this.localDirection.readyDate).toISOString();
      }

      this.$emit('save-changes', this.localDirection);
      this.localIsEditing = false;
    },
    async deleteDirection() {
      this.$emit('delete-direction', this.localDirection.id);
      this.localIsEditing = false;
    },
    async acceptDirection() {
      const userInfo = JSON.parse(localStorage.getItem('userInfo'));
      const currentDate = new Date().toISOString();

      if (!this.localDirection.bioMaterialCount) {
        this.localDirection.directionStatusId = 3; // Нет биоматериала
        this.localDirection.directionStatus = 'Нет биоматериала';
        this.localDirection.acceptedDate = null;
        this.localDirection.acceptedBy = null;
      } else {
        this.localDirection.directionStatusId = 6; // В работе
        this.localDirection.directionStatus = 'В работе';
        this.localDirection.acceptedDate = currentDate;
        this.localDirection.acceptedBy = userInfo.fullName;
      }

      // Create new status history record
      const newHistory = {
        id: null, // This will be set by the backend
        directionId: this.localDirection.id,
        dateTime: currentDate,
        directionStatusId: this.localDirection.directionStatusId,
        userFio: this.localDirection.acceptedBy,
        comment: this.localDirection.laborantComment
      };

      if (this.localDirection.directionStatusId === 3) {
        newHistory.userFio = null; // If status is 3, acceptedBy should be null
      }

      // Make sure history IDs are properly handled
      const historyWithProperId = this.localDirection.directionStatusHistory.map((history, index) => ({
        ...history,
        id: history.id || index + 1
      }));

      historyWithProperId.push(newHistory);

      const acceptRequest = {
        directionDto: {
          ...this.localDirection,
          directionStatusHistory: historyWithProperId,
        },
        acceptedBy: userInfo.fullName,
        comment: this.localDirection.laborantComment
      };

      try {
        await this.$api.post(`direction/accept/${this.localDirection.id}`, acceptRequest);
        this.$emit('accept-direction', this.localDirection);
      } catch (error) {
        console.error('Failed to accept direction:', error);
        alert('Не удалось принять направление.');
      }
    },
    updateSamplingDate() {
      const now = new Date().toISOString();
      this.localDirection.samplingDate = now;
      this.localDirection.samplingDateStr = now;
    },
    updateAge() {
      const birthDate = new Date(this.localDirection.patient.birthDate);
      const ageDifMs = Date.now() - birthDate.getTime();
      const ageDate = new Date(ageDifMs);
      this.localDirection.patient.age = Math.abs(ageDate.getUTCFullYear() - 1970);
    },
    updateSex() {
      if (this.localDirection.patient.sexDescription === 'Мужской') {
        this.localDirection.patient.sex = 1;
      } else if (this.localDirection.patient.sexDescription === 'Женский') {
        this.localDirection.patient.sex = 2;
      }
    },
    confirmDelete() {
      if (confirm('Вы уверены, что хотите удалить это направление?')) {
        this.deleteDirection();
      }
    }
  },
  watch: {
    direction(newDirection) {
      this.localDirection = { ...newDirection };
    },
    isEditing(newIsEditing) {
      this.localIsEditing = newIsEditing;
    }
  }
};
</script>

<style scoped>
.overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.patient-details {
  background-color: #fff;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
  width: 80%;
  max-width: 800px;
  max-height: 90%;
  overflow-y: auto;
}

.header {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 10px;
}

.action-button {
  background-color: #ADD8E6;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 5px;
  cursor: pointer;
  margin-left: 10px;
}

.action-button:hover {
  background-color: #8dbcd4;
}

.save-button {
  background-color: #ADD8E6;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 5px;
  cursor: pointer;
  margin-left: 10px;
}

.save-button:hover {
  background-color: #8dbcd4;
}

.delete-button {
  background-color: #ffcccc;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 5px;
  cursor: pointer;
  margin-left: 10px;
}

.delete-button:hover {
  background-color: #ff9999;
}

.patient-info {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 20px;
}

.patient-info img {
  width: 100px;
  height: 100px;
  border-radius: 50%;
  margin-bottom: 10px;
}

.patient-info h2 {
  margin: 0;
  color: #333;
}

.patient-info h3 {
  margin: 5px 0 15px;
  color: #666;
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
  background-color: #f9f9f9;
  padding: 10px;
  border-radius: 5px;
  margin-bottom: 20px;
}

.additional-info p {
  margin: 10px 0;
}

.direction-status-history {
  background-color: #f9f9f9;
  padding: 10px;
  border-radius: 5px;
}

.direction-status-history h3 {
  margin-top: 0;
}

.history-item {
  margin-bottom: 10px;
  padding: 10px;
  background-color: #fff;
  border-radius: 5px;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.1);
}

.inline-input {
  display: flex;
  align-items: center;
}

.update-button {
  background-color: #ADD8E6;
  color: white;
  border: none;
  padding: 5px 10px;
  border-radius: 5px;
  cursor: pointer;
  margin-left: 10px;
}

.update-button:hover {
  background-color: #8dbcd4;
}
</style>
