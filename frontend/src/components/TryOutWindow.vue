<template>
    <div class="main-layout">
        <Header />
        <ChatWindow v-bind:IsTryOut="true"/>
    </div>
</template>

<script>
import ChatWindow from './ChatWindow.vue';
import Header from './Header.vue';
export default {
    components: {
        ChatWindow,
        Header
    },
};

</script>

<style scoped>
.main-layout {
    display: flex;
    flex-direction: column;
    height: 100vh;
    width: 100vw;
}
</style>
