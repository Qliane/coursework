import { createApp, reactive } from './vue.esm-browser.js'


class SortedArray {
    data = [];
    handler = (a, b) => { a - b };
    constructor(handler, data = []) {
        this.data = data.sort(handler);
        this.handler = handler;
    }

    push(item) {
        this.data.push(item);
        this.update();
    }

    getIndexById(id) {
        return this.data.findIndex((value) => value.id == id)
    }

    update() {
        this.data.sort(this.handler);
    }

    remove(index) {
        this.data.splice(index, 1)
    }

    #insert(item, index) {
        const lenght = this.data.length;
        const temp = this.data[lenght - 1];
        for (let i = index + 1; i < lenght; i++) {
            this.data[i] = this.data[i - 1];
        }
        this.data[index] = item;
        this.data.push(temp);
    }
}

window.app = createApp({
    setup() {
        return {
        }
    }
})

app.component('list-item', {
    props: {
        t: String,
        index: Number,
        id: Number,
        order: Number,
        moveHandler: Function,
        mouseoverHandle: Function,
        touchMove: Function,
        deleteItem: Function,
        item: Object,
        createToolTip: Function,
    },
    methods: {
        dwn(e) {
            this.moveHandler(this, e.x, e.y);
        },
        dwn_touch(e) {
            const touch = e.touches[0];
            this.moveHandler(this, touch.pageX, touch.pageY);
        },
        over(e) {
            this.mouseoverHandle(this);
        },

        showColorTooltip() {
            const colorElement = this.$el.querySelector('.color');
            const x = colorElement.offsetLeft;
            const y = colorElement.offsetTop;
            this.createToolTip(this.id, x + colorElement.offsetWidth / 2, y);
        },
        edit(e) {
            const value = prompt("Введите новое значение", this.t);
            if (value !== null) {
                this.item.text = value;
                updateItem(this.item);
            }
        },
        onSelect(){
            this.item.selected = (!this.item.selected ? 1 : 0)
            updateItem(this.item);
        },
    },

    template: `
    <div class="item list-item" v-on:mouseover="over" v-bind:style="{backgroundColor: item.color}">
        <div className="content">
            <div className="draggable" v-on:touchstart="dwn" v-on:mousedown="dwn" v-on:touchmove.prevent="touchMove">
            </div>
            <button v-bind:data-state="item.selected" v-on:click="onSelect"  v-bind:data-id="id" class="checkbox">

            </button>
            <div class="text">
                {{t}}
            </div>
        </div>
        <div className="buttons">
            <div class="list-item-button delete">
                <button class="delete-button" data-id="0" v-on:click="deleteItem">
                </button>
            </div>
            <div class="list-item-button color">
                <button class="color-button" data-id="0" v-on:click="showColorTooltip">
                </button>
            </div>
            <div class="list-item-button edit">
                <button class="edit-button" data-id="0" v-on:click="edit">
                </button>
            </div>
        </div>
    </div>`
})

app.component('tooltip-color', {
    props: {
        data: Object,
        onCheckHandler: Function
    },

    data() {
        const width_t = 25;
        const height_t = 17;
        const width = 277;
        const height = 49;
        const { x, y } = this.data;
        const W = window.outerWidth;
        let left = x - width / 2;
        let top = (y - height - height_t);
        let triangleOffset = 0;

        if (x + width / 2 > W) {
            const dw = x + width / 2 - W;
            left -= dw;
            triangleOffset = dw;
        }

        return {
            triangleStyle: {
                left: (width / 2 - width_t / 2 + triangleOffset) + "px",
            },
            style: {
                left: left + "px",
                top: top + "px"
            },
            colors: [
                "#c6e191",
                "#e19191",
                "#e1d091",
                "#91d8e1",
                "#9991e1",
                "#e191bc",
                null,
            ]
        }
    },

    methods: {
        onCheck(color) {
            console.log(color);
            this.onCheckHandler(this.data.id, color);
        },
    },


    template: `
    <div class="tooltip color" v-bind:style="style">
        <div className="colors">
            <button class="color" v-for="color in colors" v-bind:style="{background: color}" v-bind:class="{ 'empty': color == null }" v-on:click="()=>{onCheck(color)}"></button>
        </div>
        <div className="triangle" v-bind:style="triangleStyle">
        </div>
    </div>`,
})


const dragAndDropStates = {
    NONE: 0,
    CLICKED: 1,
    RELEASED: 2
};

const data = reactive(new SortedArray((a, b) => b.order - a.order, []));


app.component('list', {
    props: {
        checked: String
    },

    data() {
        return {
            items: (data),

            tooltips: [
            ],

            selectState: dragAndDropStates.NONE,
            lastClickPosition: {
                x: 0,
                y: 0,
            },
            draggableElementIndex: null,
        }
    },

    methods: {
        select(items) {
            return items.filter((item) => (item.selected == (this.checked == "1")));
        },

        selectItem(element) {
            if (this.selectState == dragAndDropStates.CLICKED)
                this.draggableElementIndex = element.index;
        },

        localIdexToDataIndex(i){
            let index = 0;
            for (const element of this.items.data) {
                if (element.selected == (this.checked == "1")) {
                    i--;
                }
                if (i < 0) return index;
                index++;
            }
            return null;
        },

        switch(i1, i2) {
            const i1_ = this.localIdexToDataIndex(i1);
            const i2_ = this.localIdexToDataIndex(i2);
            const temp = this.items.data[i1_].order;
            this.items.data[i1_].order = this.items.data[i2_].order;
            this.items.data[i2_].order = temp;
            this.items.update();

            updateItem(this.items.data[i1_]);
            updateItem(this.items.data[i2_]);

            if (i2 == this.draggableElementIndex) {
                this.draggableElementIndex = i1;
            }
            else if (i1 == this.draggableElementIndex) {
                this.draggableElementIndex = i2;
            }
        },

        onMoveStart(element, x, y) {
            this.lastClickPosition = {
                x: x,
                y: y,
            };
            console.log(x, y);
            this.selectState = dragAndDropStates.CLICKED;
            this.selectItem(element);
        },

        onMouseOver(element) {
            if (this.selectState == dragAndDropStates.RELEASED) {
                this.switch(element.index, this.draggableElementIndex);
            }
        },

        onMouseMove(e) {
            const dndStartLenght = 5; // количество пикселей начала drag and drop
            const isReady = (p1, p2) => {
                return (Math.abs(p1.x - p2.x) >= dndStartLenght) ||
                    (Math.abs(p1.y - p2.y) >= dndStartLenght)
            };

            if (isReady(this.lastClickPosition, { x: e.x, y: e.y })) {
                switch (this.selectState) {
                    case dragAndDropStates.CLICKED:
                        this.selectState = dragAndDropStates.RELEASED;
                        break;
                    default:
                        break;
                }
            }
        },

        mouseupHandler(e) {
            if (this.selectState == dragAndDropStates.RELEASED) {
                console.log("send to server");
            }
            this.selectState = dragAndDropStates.NONE;
        },
        deleteItem(index) {
            const i = this.localIdexToDataIndex(index);
            if (confirm("Вы действительно хотите удалить запись?")) {
                removeItem(data.data[i])
                this.items.remove(i);
            }
        },

        touchMove(e) {
            const touch = e.touches[0];
            const element = document.elementFromPoint(touch.clientX, touch.clientY);
            const item = element.closest(".list-item");
            if (item) {
                this.selectState = dragAndDropStates.RELEASED
                const id = Array.from(this.$el.querySelectorAll('.list-item')).indexOf(item);
                console.log(id);
                this.switch(id, this.draggableElementIndex);
            }
        },

        deleteToolTip(id) {
            const i = this.tooltips.findIndex((el => el.id == id))
            this.tooltips.splice(i, 1);
        },

        createToolTip(id, x, y) {
            const i = this.tooltips.findIndex((el => el.id == id))
            if (i == -1) {
                this.tooltips.push({ id, x, y });
            } else {
                this.deleteToolTip(id);
            }
        },

        tooltipHandler(id, color) {
            console.log(id, color);
            const item = this.items.data.find((v) => v.id == id);
            if (item !== undefined) {
                item.color = color;
                updateItem(item);
                this.deleteToolTip(id);
            }

        },

    },

    template: `
    <div className="items" v-on:mouseup="mouseupHandler" v-on:mouseleave="mouseupHandler" v-on:touchend="mouseupHandler" v-on:mousemove="onMouseMove" v-on:touchmove="onMouseMove">
        <div class="list">
            <div v-if="select(items.data).length == 0">
                <div v-if="checked == '0'"> У вас нет активных задач. </div>
                <div v-else> Когда вы выполните задачу, она будет здесь </div>
            </div>
            <list-item v-for="(item, index) in select(items.data)" v-bind:key="item.id" v-bind:createToolTip="createToolTip" v-bind:deleteItem="deleteItem.bind(this, index)" v-bind:item="item" v-bind:index="index" v-bind:touchMove="touchMove" v-bind:id="item.id" v-bind:order="item.order" v-bind:t="item.text" v-bind:mouseoverHandle="onMouseOver" v-bind:moveHandler="onMoveStart"></list-item>
        </div>
        <tooltip-color v-for="(item, index) in tooltips" v-bind:key="item.id" v-bind:data="item" v-bind:onCheckHandler="tooltipHandler"></tooltip-color>
    </div>
    `,
})


app.mount('#app');

function updateItem(item) {
    const token = document.querySelector("input[name='__RequestVerificationToken']").value;
    const formData = new FormData();
    formData.append('Id', item.id);
    formData.append('Order', item.order);
    formData.append('Color', item.color);
    formData.append('Selected', item.selected);
    formData.append('Text', item.text);

    fetch("?handler=Update", {
        method: "POST",
        headers: {
            "RequestVerificationToken": token
        },
        body: formData,
    }).then(v => v.json().then(object => {
        console.log(object);
    }));
}

function removeItem(item) {
    const token = document.querySelector("input[name='__RequestVerificationToken']").value;
    const formData = new FormData();
    formData.append('Id', item.id);

    fetch("?handler=Remove", {
        method: "POST",
        headers: {
            "RequestVerificationToken": token
        },
        body: formData,
    }).then(v => v.json().then(object => {
        console.log(object);
    }));
}


window.addEventListener('load', () => {
    const url = window.location.href;
    const id = url.match(/\/List\/(\d+)/)[1];

    const createTaskButton = document.getElementById('button-create-task');

    createTaskButton?.addEventListener('click', e => {
        let text = prompt("Текст новой задачи");
        if (text == null) return;
        text = encodeURIComponent(text);
        fetch("?handler=AddTask&Text=" + text, {
            method: "GET"
        }).then((value) => {
            value.json().then(object => {
                if (object.color == "#000000") {
                    object.color = null;
                }
                console.log(object);
                data.push(object);
            })
        })
        e.preventDefault();
    })

    fetch("?handler=GetTasks", {
        method: "GET"
    }).then((value) => {
        value.json().then(object => {
            for (const item of object) {
                if (item.color == "#000000") {
                    item.color = null;
                }
            }
            data.data = object;
            data.update();
        })
    })
})