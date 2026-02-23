import {useEffect,useState} from 'react';


function Items(){
    const[items, setItems] = useState([]);
    useEffect(()=>{
        fetch("https://localhost:7293/api/Item")
        .then(res=>res.json())
        .then(data=>setItems(data))
        .catch(err=>console.log("Error:",err))
    },[])


    return(
        <div>
            <h2>Item List</h2>
            <ul>
                {items.map(p=>(
                    <li key={p.id}>{p.title},{p.content},{p.description},{p.price},{p.category}</li>

                ))}
            </ul>
        </div>
    );
}

export default Items;

