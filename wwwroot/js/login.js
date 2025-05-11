// @ts-check

window.addEventListener('load', ()=>{
    const tm = new ToastManager();

    /** @type {HTMLFormElement} */
    // @ts-ignore
    const form = document.getElementById('form');

    form?.addEventListener('submit', /** @type {SubmitEvent} */(e)=>{
        const formData = new FormData(form);
        const s = formData.get('login')?.toString();
        
        let isValid = true;

        if(isValid){

        }else{
            tm.addToast("Ошибочка вышла", 0);
            e.preventDefault();
        }

    })
})