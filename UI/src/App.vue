<template>
  <div id="app" class="uk-container uk-height-viewport">
    <button @click="showLogin = true" class="uk-button">Pokaż logowanie</button>
    <button @click="showLogin = false" class="uk-button">Schowaj logowanie</button>
    <button @click="showCharacterSelection = true" class="uk-button">Pokaż wybór postaci</button>
    <button @click="showCharacterSelection = false" class="uk-button">Schowaj wybór postaci</button>
    <LoginComponent v-bind:showWindow="showLogin" />
    <CharacterSelectionComponent v-bind:showWindow="showCharacterSelection" />
  </div>
</template>

<script>
import LoginComponent from "./components/LoginComponent.vue";
import CharacterSelectionComponent from "./components/CharacterSelectionComponent.vue";

export default {
  name: "App",
  components: {
    LoginComponent,
    CharacterSelectionComponent
  },
  data() {
    return { showLogin: false, showCharacterSelection: false };
  }
};
</script>
