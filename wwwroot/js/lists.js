

window.addEventListener('load', ()=>{
    const titleButton = document.getElementById('titleButton');
    const addButtons = document.getElementsByClassName('add-list-button');
    const token = document.querySelector("input[name='__RequestVerificationToken']").value;

    if(titleButton === null) return;

    const handler = (e)=>{
        const formData = new FormData();
        const name = prompt("Введте название записи");
        if(name == null) return;
        
        formData.append('Name', name);
        
        fetch('/?handler=AddList', {
            method: 'POST',
            headers: {
                "RequestVerificationToken": token
             },
            body: formData
        }).then((value)=>{
            value.json().then((object)=>{
                if(object.id !== undefined){
                    window.location.reload();
                }
            })
        })
        e.preventDefault();
    }
    
    for (const button of addButtons) {
        button.addEventListener('click', handler);
    }

    titleButton.addEventListener('click', handler);
});