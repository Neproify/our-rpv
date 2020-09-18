<template>
  <div id="app" class="uk-container uk-height-viewport">
    <button @click="showLogin = true" class="uk-button">Pokaż logowanie</button>
    <button @click="showLogin = false" class="uk-button">Schowaj logowanie</button>
    <button @click="showCharacterSelection = true" class="uk-button">Pokaż wybór postaci</button>
    <button @click="showCharacterSelection = false" class="uk-button">Schowaj wybór postaci</button>
    <button @click="showEquipment = true" class="uk-button">Pokaż ekwipunek</button>
    <button @click="showEquipment = false" class="uk-button">Schowaj ekwipunek</button>
    <button @click="showCharacterCreator = true" class="uk-button">Pokaż tworzenie postaci</button>
    <button @click="showCharacterCreator = false" class="uk-button">Schowaj tworzenie postaci</button>
    <LoginComponent v-bind:showWindow="showLogin" />
    <CharacterSelectionComponent v-bind:showWindow="showCharacterSelection" />
    <EquipmentComponent v-bind:showWindow="showEquipment" />
    <CharacterCreatorComponent v-bind:showWindow="showCharacterCreator" />
  </div>
</template>

<script>
import LoginComponent from "./components/LoginComponent.vue";
import CharacterSelectionComponent from "./components/CharacterSelectionComponent.vue";
import EquipmentComponent from "./components/EquipmentComponent.vue";
import CharacterCreatorComponent from "./components/CharacterCreatorComponent.vue";

export default {
  name: "App",
  components: {
    LoginComponent,
    CharacterSelectionComponent,
    EquipmentComponent,
    CharacterCreatorComponent,
  },
  data() {
    return {
      showLogin: false,
      showCharacterSelection: false,
      showEquipment: false,
      showCharacterCreator: false,
    };
  },
};
</script>
