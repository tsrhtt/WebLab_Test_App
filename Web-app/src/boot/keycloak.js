import Keycloak from 'keycloak-js'
import { boot } from 'quasar/wrappers'

const keycloakConfig = {
  url: process.env.KEYCLOAK_HOST,
  realm: process.env.KEYCLOAK_REALM,
  clientId: process.env.KEYCLOAK_CLIENT_ID,
  onLoad: process.env.KEYCLOAK_ON_LOAD
}

const keycloak = new Keycloak(keycloakConfig)

export default boot(({ app, router, store }) => {
  return new Promise(async (resolve, reject) => {
    await keycloak.init({ onLoad: keycloakConfig.onLoad, checkLoginIframe: false })
      .then((authenticated) => {
        if (authenticated) {
          console.log("Authenticated");
        } else {
          window.location.reload();
        }
      })
      .catch((error) => {
        reject(error)
      })

    const token = keycloak.token;

    console.log(token);

    // Распарсивание токена и сохранение информации о пользователе
    const userInfo = {
      user: keycloak.tokenParsed.preferred_username,
      fullName: keycloak.tokenParsed.name,
      userGroup: keycloak.tokenParsed.groups,
      clientURL: keycloak.tokenParsed['allowed-origins'] || [] // Проверка наличия поля allowed-origins
    }
    localStorage.setItem('userInfo', JSON.stringify(userInfo))
    localStorage.setItem('token', token)

    resolve()
  })
})

export { keycloak }
