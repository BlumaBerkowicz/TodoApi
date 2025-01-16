import axios from 'axios';
axios.defaults.baseURL = process.env.API;
const addErrorInterceptor= () => {
  axios.interceptors.response.use(
    (response) => {
      console.log('Successful request:', response.status, response.data);
      return response;
    },
    (error) => {
      console.error('Request error:', error.response ? error.response.status : error.message, error.response ? error.response.data : error);
      return Promise.reject(error); 
    }
  );
};
addErrorInterceptor();
export default {  

  getTasks: async () => {
    const result = await axios.get() 
    return result.data;
  },

  addTask: async(name)=>{
    addErrorInterceptor();
    const result = await axios({
      method: 'post',
      url: 'item',
      data: {name: name}
    });
    console.log('add task- data', result.status) 
    return result.data;
  },

  setCompleted: async(id, isComplete)=>{
    addErrorInterceptor();
    const result = await axios({
      method: 'put',
      url: `item/{id}`,
      data: {
        id: id,
        isComplete:isComplete
            }
    });
    console.log('put task- data', result.status) 
    return result.data;
  },

  deleteTask:async(id)=>{
    addErrorInterceptor();
    const result = await axios({
      method: 'delete',
      url: `item/${id}`
  });
}
}
