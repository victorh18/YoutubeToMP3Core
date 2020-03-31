import React, { useState, useEffect, useCallback } from 'react';
import { Row, Col, Input, Select,Typography, Spin, Button } from 'antd';
import util from '../util/util';
import { LoadingOutlined, VideoCameraOutlined, UnorderedListOutlined } from '@ant-design/icons';
import VideoPreview from './VideoPreview';
import FormatSelect from './FormatSelect';
import ytService from '../services/youtubeService';
const { debounce, isURL } = util;
const {Title} = Typography;

const antIcon = <LoadingOutlined style={{ fontSize: 24 }} spin />;

export default function VideoDownloader(props) {
    const [url, setUrl] = useState('');
    const [format, setFormat] = useState(2);
    const [meta, setMeta] = useState(null);
    const [loadingMeta, setLoadingMeta] = useState(false);

    const dGetMeta = useCallback(debounce(getMeta, 1000), []);

    function getMeta(urlp) {
        setLoadingMeta(true);

        ytService.getMetadata(urlp).then(data => {
            setMeta(data);
        }).finally(() => {
            setLoadingMeta(false);
        });
    };

    function downloadVid(urlp) {
        ytService.download(urlp, format);
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
                                <VideoPreview Meta={meta}></VideoPreview>
                            </Col>
                        </Row>
                    }

                    {!meta &&
                        <Row style={{ marginTop: '10px', marginBottom: '10px' }}>
                            <Col span={20} offset={2}>
                                <Title >Inserta el URL del video</Title>
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
                                <Button onClick={() => { downloadVid(url) }} size={'large'} type="primary">Descargar</Button>
                            </Col>
                        </Row>
                    }

                    <Input size="large" onChange={({ target: { value } }) => setUrl(value)} value={url}
                        addonAfter={<FormatSelect value={format} onChange={props => { setFormat(props) }}></FormatSelect>}
                        placeholder="URL del video"></Input>
                </Col>
            </Row>
        </React.Fragment>
    );
}