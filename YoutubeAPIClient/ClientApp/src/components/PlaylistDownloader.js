import React, { useState, useEffect, useCallback } from 'react';
import { Row, Col, List, Avatar, Input, Select,Typography, Spin, Button } from 'antd';
import util from '../util/util';
import { LoadingOutlined, VideoCameraOutlined, UnorderedListOutlined } from '@ant-design/icons';
import VideoPreview from './VideoPreview';
import FormatSelect from './FormatSelect';
import ytService from '../services/youtubeService';
import * as moment from 'moment';
const { debounce, isURL } = util;
const {Title} = Typography;

const antIcon = <LoadingOutlined style={{ fontSize: 24 }} spin />;

export default function PlaylistDownloader(props) {
    const [url, setUrl] = useState('');
    const [format, setFormat] = useState(2);
    const [meta, setMeta] = useState(null);
    const [loadingMeta, setLoadingMeta] = useState(false);

    const dGetMeta = useCallback(debounce(getMeta, 1000), []);

    function getMeta(urlp) {
        setLoadingMeta(true);

        ytService.getPlaylistMeta(urlp).then(data => {
            console.log('meta');
            console.log(data);
            setMeta(data);
        }).finally(() => {
            setLoadingMeta(false);
        });
    };

    function downloadList(urlp) {
        ytService.downloadPlaylist(urlp, format);
    }

    useEffect(() => {
        if (isURL(url)) {
            dGetMeta(url);
        }
    }, [url]);

    return (
        <React.Fragment>
            <Row>
                <Col span={20} offset={2}>
                    {!loadingMeta && meta &&
                        <Row style={{ marginTop: '10px', marginBottom: '10px' }}>
                            <Col span={12} offset={6}>
                                <h1>{meta.title}</h1>
                                <h5>{meta.author}</h5>
                            </Col>
                        </Row>
                    }

                    {!meta &&
                        <Row style={{ marginTop: '10px', marginBottom: '10px' }}>
                            <Col span={20} offset={2}>
                                <Title >Inserta el URL del playlist</Title>
                            </Col>
                        </Row>
                    }

                    {loadingMeta &&
                        <Row style={{ marginTop: '10px', marginBottom: '10px' }}>
                            <Col span={20} offset={2}>
                                <Spin size='large' indicator={antIcon} />
                            </Col>
                        </Row>
                    }

                    {meta &&
                        <Row style={{ marginTop: '10px', marginBottom: '10px' }}>
                            <Col span={20} offset={2}>
                                <Button onClick={() => { downloadList(url) }} size={'large'} type="primary">Descargar</Button>
                            </Col>
                        </Row>
                    }

                    <Input size="large" onChange={({ target: { value } }) => setUrl(value)} value={url}
                        addonAfter={<FormatSelect value={format} onChange={props => { setFormat(props) }}></FormatSelect>}
                        placeholder="URL del playlist"></Input>


                    {!loadingMeta && meta &&
                        <List
                            itemLayout="horizontal"
                            dataSource={meta.videos}
                            renderItem={item => (
                            <List.Item>
                                <List.Item.Meta
                                avatar={<Avatar shape="square" size="large" src={item.thumbnails.maxResUrl} />}
                                title={item.title}
                                description={`${item.author} - ${moment(item.uploadDate).format('DD/MM/YYYY')}`}
                                />
                            </List.Item>
                            )}
                        />
                    }


                </Col>
            </Row>
        </React.Fragment>
    )
}