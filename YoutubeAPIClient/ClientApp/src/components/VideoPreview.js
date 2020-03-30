import React, {useEffect} from 'react';
import * as moment from 'moment';
import { Card } from 'antd';


function VideoPreview(props) {
    const {Meta} = props;

    useEffect(() => {
        console.log(Meta);
    });

    return (
        <Card 
            hoverable
            cover={<img alt="thumbnail" src={Meta.thumbnails.maxResUrl}/>}
        >
            <h1>{Meta.title}</h1>
            <span>Publicado: {moment(Meta.uploadDate).format('DD/MM/YYYY')}</span>
        </Card>
        
    );
}

export default VideoPreview