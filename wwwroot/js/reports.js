window.addEventListener('load', ()=>{
    const buttons = document.getElementsByClassName("delete-report-button");
    for (const button of buttons) {
        button.addEventListener('click', (e)=>{
            if(confirm("Вы действительно хотите удалить отчёт?") == false) e.preventDefault();
        })
    }
})