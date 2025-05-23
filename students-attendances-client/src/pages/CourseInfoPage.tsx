import PageCard from "@components/pagecard/PageCard";
import StudentsList from "@components/students/StudentsList";
import type { CourseInfoModel } from "@core/models/course-models";
import { useCourseStore, useLessonStore } from "@core/store/store";
import { LucideCircleArrowLeft } from "lucide-react";
import { useEffect, useState, type JSX } from "react";
import { Button, Col, Form, Image, ListGroup, Row, Stack } from "react-bootstrap";
import { useNavigate, useParams } from "react-router-dom";

export default function CourseInfoPage(): JSX.Element {
	const { courses, students, fetchStudents } = useCourseStore()
	const { setCourse } = useLessonStore()

	const [ current, setCurrent ] = useState<CourseInfoModel>()
	const { id } = useParams() 

	const navigate = useNavigate()

	const lessonsHandler = () => {
		if (current) {
		setCourse(current)
		navigate(`/lessons`)
		}
	}

	useEffect(() => {
		if (!courses || courses.length <= 0 || !id) {
			navigate('/dashboard')
			return
		}
		setCurrent(courses.find(item => item.externalId == parseInt(id)));
		(async () => {
			fetchStudents(parseInt(id))
		})().catch(error => console.log(error))
	// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [])

	if (!current) return <></>
	return (
		<PageCard md={12} lg={12}>
			<Stack gap={3} className="w-100 p-2 p-md-4 pb-md-5 shadow-sm rounded bg-white bg-opacity-75">
				<div className='d-flex flex-row justify-content-center align-items-center'>
					<h3 className="m-0" style={{textTransform: 'uppercase'}}>Информация о курсе: {current.shortName}</h3>
				</div>
				<div className="d-flex justify-content-between align-items-center w-100">
					<div className='gradient-input-wrapper p-1' onClick={() => navigate(-1)} style={{
						cursor: 'pointer'
					}}>
						<LucideCircleArrowLeft color='white' size={26}/>
					</div>
					<div className="gradient-input-wrapper" style={{
						maxHeight: '36px'
					}}>
						<Button onClick={() => lessonsHandler()} style={{
							backgroundColor: 'transparent', 
							border: '0',
							fontSize: '13px'
						}}>Перейти к урокам</Button>
					</div>
				</div>
				<div>
					<Row className="justify-content-center gx-3 gy-3">
						<Col className="" sm='12' md='12' lg='4' xl='3'>
							<div className="py-4 shadow rounded bg-white bg-opacity-50">
								<Row className="m-0">
									<Col xs='12' sm='6' md='6' lg='12'>
										<Form.Group>
											<Form.Label className="w-100 mx-2 mb-0 text-start">Полное название:</Form.Label>
												<div className="gradient-input-wrapper mb-3">
													<Form.Control style={{ height: '32px'}}
														type="text"
														className="gradient-input"
														defaultValue={current.fullName}
														readOnly
													/>
												</div>
										</Form.Group>
									</Col>
									<Col xs='12' sm='6' md='6' lg='12'>
										<Form.Group>
											<Form.Label className="w-100 mx-2 mb-0 text-start">Период:</Form.Label>
											<div className="gradient-input-wrapper mb-3">
												<Form.Control style={{ height: '32px'}}
													type="text"
													className="gradient-input"
													defaultValue={`${formatDate(current.startDate)} - ${formatDate(current.endDate)}`}
													readOnly
												/>
											</div>
										</Form.Group>
									</Col>
								</Row>
							</div>
							
						</Col>
						
						<Col className="" sm='12' md='12' lg='8' xl='9'>
							<div className="p-4 shadow rounded bg-white bg-opacity-50">
								<div className="mb-3 ">
									<h5 className='fw-normal text-start fs-6'>Список учителей курса:</h5>
									<div style={{
										maxHeight: '260px',
										overflow: 'auto'
									}}>
										<ListGroup className="">
										{
											current.teachers.map((item, index) => (
												<ListGroup.Item style={{backgroundColor: 'transparent'}} key={`teacher-${index}`}>
													<div className='d-flex flex-row align-items-center gap-3'>
														<div>
															<Image src={item.imageUrl} roundedCircle width={30} height={30} />
														</div>
														<div>
															<span>{item.firstName} {item.lastName}</span>
														</div>
														<div>
															<span>{item.email}</span>
														</div>
													</div>
												</ListGroup.Item>
											))
										}
										</ListGroup>
									</div>
								</div>
								<div className="my-4" style={{
									background: 'linear-gradient(90deg, #ff6f61, #6a11cb)',
									width: '96%',
									height: '2px',
									margin: 'auto'
								}}/>
								<div style={{
									maxHeight: '440px',
									overflow: 'auto'
								}}>
									<StudentsList students={students} />
								</div>
							</div>
						</Col>
					</Row>
				</div>
			</Stack>
		</PageCard>
	)
}

function formatDate(isoData: string | undefined | null): string {
	if (isoData) {
		const date = new Date(isoData);
		return new Intl.DateTimeFormat("ru-RU").format(date);
	}
	return '...'
}