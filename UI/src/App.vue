<template>
  <div id="app" class="uk-container uk-height-viewport">
    <LoginComponent />
    <CharacterSelectionComponent />
    <EquipmentComponent />
    <CharacterCreatorComponent />
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
      debug: false,
    };
  },
};
</script>
