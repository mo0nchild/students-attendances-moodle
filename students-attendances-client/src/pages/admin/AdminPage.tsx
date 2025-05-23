import StudentsWithoutRfid from "@components/admin/StudentsWithoutRfid";
import StudentsWithRfid from "@components/admin/StudentsWithRfid";
import Loading from "@components/loading/Loading";
import PageCard from "@components/pagecard/PageCard";
import { useDeviceComm } from "@core/hooks/useDeviceComm";
import type { RfidMarkerDetailsModel, UserInfoModel } from "@core/models/user-models";
import { adminService } from "@services/AdminService";
import { AxiosError } from "axios";
import { RefreshCcw } from "lucide-react";
import { useEffect, useRef, useState, type JSX } from "react";
import { Button, Stack } from "react-bootstrap";

type StudentsWithoutRfidSetting = {
	student: UserInfoModel
	value: string
}

export default function AdminPage(): JSX.Element {

	const [ scanning, setScanning ] = useState<boolean>(false)
	const [ rfidMarkers, setRfidMarkers ] = useState<RfidMarkerDetailsModel[]>([])
	const [ students, setStudents ] = useState<UserInfoModel[]>([])
	const [ isLoading, setLoading ] = useState<boolean>(false)

	const [ selectedWithout, setSelected ] = useState<UserInfoModel>()
	const selectedWithoutRef = useRef<UserInfoModel | undefined>(undefined)

	const [ selectedWith, setSelectedWith ] = useState<StudentsWithoutRfidSetting>()

	const { isConnected } = useDeviceComm({
		enabled: scanning,
		onData: data => {
			if (!data) return
			console.log(data)
			if (selectedWithoutRef.current) {
				setSelectedWith({
					student: selectedWithoutRef.current,
					value: data
				})
				setSelected(selectedWithoutRef.current = undefined)
			}
			console.log(selectedWithoutRef.current)
		},
		onError: error => {
			console.log(error)
			if (error) alert(error)
				
			setScanning(false)
			setSelected(undefined)
		}
	})
	const cancelScanning = () => {
		setSelected(selectedWithoutRef.current = undefined)
	}
	const handleSelectedStudentWithout = (student: UserInfoModel | undefined) => {
		selectedWithoutRef.current = student
		setSelected(student)
	}
	const saveRfidMarker = async () => {
		if (!selectedWith) return
		try {
			const uuid = await adminService.setRfidMarkerToStudent({
				rfidValue: selectedWith.value,
				userId: selectedWith.student.externalId, 
			})
			console.log(uuid)
			setSelectedWith(undefined)
			refreshPage()
		}
		catch (error) {
			if (error instanceof AxiosError) {
				alert(error.response ? `Ошибка при выполнении запроса` : `Сервер не доступен`)
			}
			console.log(error)
		}
	}
	const cancelSaveRfidMarker = () => {
		setSelectedWith(undefined)
	}

	const deleteRfidMarker = async (rfidMarker: RfidMarkerDetailsModel | undefined) => {
		if (!rfidMarker) return;
		try {
			await adminService.deleteRfidMarker(rfidMarker.uuid)
			refreshPage()
		}
		catch (error) {
			if (error instanceof AxiosError) {
				alert(error.response ? `Ошибка при выполнении запроса` : `Сервер не доступен`)
			}
			console.log(error)
		}
	}

	const refreshPage = () => {
		(async() => {
			setLoading(true);
			setStudents((await adminService.getUsersWithoutRfidMarkers()).data)
			setRfidMarkers((await adminService.getAllRfidMarkers()).data)
			setLoading(false)
		})().catch(error => {
			console.log(error)
			setLoading(false)
		})
	}

	useEffect(() => refreshPage(), [])

  return (
		<PageCard>
			<Stack gap={3} className="w-100 p-2 p-md-4 pb-md-5 shadow rounded bg-white bg-opacity-75">
				<div className='d-flex flex-row justify-content-between align-items-center mb-4'>
					<h3 className="m-0" style={{textTransform: 'uppercase'}}>Панель администратора</h3>
					<div className="d-flex flex-row gap-2">
						<div className="gradient-input-wrapper">
							<Button className={`border-0 bg-white`} 
								style={{
									height: '34px',
									borderRadius: '10px'
								}}
								onClick={() => refreshPage()}>
								<RefreshCcw size={15} color='black'/>
							</Button>
						</div>
						<div className="gradient-input-wrapper">
							<Button className={`border-0 ${!isConnected ? 'bg-transparent' : 'bg-white'}`} 
								style={{
									borderRadius: '10px',
									fontSize: '14px',
									color: !isConnected ? 'white' : 'black'
								}}
								onClick={() => setScanning(!scanning)}>
								{ !isConnected ? 'Подключиться' : 'Отключиться' }
							</Button>
						</div>
					</div>
				</div>
				<div className='d-flex flex-row justify-content-start align-items-center'>
					<h5 className="m-0">Списки студентов:</h5>
				</div>
				<Loading isLoading={isLoading}>
					<div className="mb-3">
						{
							selectedWith 
								?	<div className='border-1 rounded d-flex flex-column align-items-center mb-4 gap-3
											bg-white bg-opacity-75 p-4 shadow'>
										<h6>Новое значение маркера</h6>
										<div>
											<p className="m-0">{`${selectedWith.student.firstName} ${selectedWith.student.lastName}`}</p>
											<p className="m-0">Значение RFID: &nbsp;
												<span style={{fontStyle: 'italic'}}>{selectedWith.value}</span>
											</p>
										</div>
										<div className='d-flex flex-row justify-content-center gap-4'>
											<div className="gradient-input-wrapper">
												<Button className='bg-transparent border-0' 
													style={{
														fontSize: '14px',
														width: '90px'
													}}
													onClick={() => saveRfidMarker()}>
													Сохранить
												</Button>
											</div>
											<div className="gradient-input-wrapper">
												<Button className='bg-transparent border-0' 
													style={{
														fontSize: '14px',
														width: '90px'
													}}
													onClick={() => cancelSaveRfidMarker()}>
													Отмена
												</Button>
											</div>
										</div>
										
									</div>
								: <></> 
						}
						{
							!selectedWithout 
								? <div>
										<h6>Студенты без RFID маркеров</h6>
										<StudentsWithoutRfid students={students} 
											disabled={!isConnected}
											onSelected={handleSelectedStudentWithout}/>
									</div>
								:	<div className='d-flex flex-column align-items-center border-1 rounded mb-4
											bg-white bg-opacity-75 p-4 shadow'>
										<div style={{ 
											width: '120px', 
											height: '100px', 
											overflow: 'hidden' 
										}}>
											<img src="/animation2.gif"
												alt="GIF" 
												style={{ 
													width: '100%', 
													height: 'auto', 
													objectFit: 'cover',
													objectPosition: 'top'
												}} 
											/>
										</div>
										<div className="d-flex flex-column gap-3">
											<p className='fs-5 m-0'>Сканирование...</p>
											<div className="gradient-input-wrapper">
												<Button className='bg-transparent border-0' 
													style={{
														fontSize: '14px',
														width: '90px'
													}} onClick={() => cancelScanning()}>Отмена</Button>
											</div>
										</div>
										
									</div>
						}
					</div>
					<div className="mb-3">
						<h6>Студенты с RFID маркерами</h6>
						<StudentsWithRfid students={rfidMarkers} onSelected={deleteRfidMarker}/>
					</div>
				</Loading>
				
			</Stack>
		</PageCard>
	)
}